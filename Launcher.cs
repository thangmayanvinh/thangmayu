using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Lottery5DSimulator
{
    static class Program
    {
        private static HttpListener _listener;
        private static int _port;
        private static readonly Dictionary<string, byte[]> _cachedFiles = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        private static NotifyIcon _trayIcon;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                LoadEmbeddedAssets();
                StartWebServer();
                LaunchAppWindow();
                SetupTrayIcon();

                Application.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Khởi động 5D Lottery Simulator thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopWebServer();
            }
        }

        private static void LoadEmbeddedAssets()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("dist.zip"))
            {
                if (stream == null)
                {
                    // Fallback to local dist.zip if not embedded
                    if (File.Exists("dist.zip"))
                    {
                        using (var fs = File.OpenRead("dist.zip"))
                        {
                            ExtractZipToMemory(fs);
                            return;
                        }
                    }
                    throw new Exception("Không tìm thấy tệp tài nguyên ứng dụng dist.zip.");
                }

                ExtractZipToMemory(stream);
            }
        }

        private static void ExtractZipToMemory(Stream stream)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name)) continue; // Directory

                    using (var entryStream = entry.Open())
                    using (var ms = new MemoryStream())
                    {
                        entryStream.CopyTo(ms);
                        var key = entry.FullName.Replace('\\', '/').TrimStart('/');
                        _cachedFiles[key] = ms.ToArray();
                    }
                }
            }
        }

        private static void StartWebServer()
        {
            // Pick a fixed preferred port or dynamic port
            int[] preferredPorts = new int[] { 52525, 52526, 52527, 52528, 52529 };
            foreach (var p in preferredPorts)
            {
                try
                {
                    var l = new HttpListener();
                    l.Prefixes.Add("http://127.0.0.1:" + p + "/");
                    l.Start();
                    _listener = l;
                    _port = p;
                    break;
                }
                catch
                {
                    // port in use, try next
                }
            }

            if (_listener == null)
            {
                // Fallback to random free port
                var l = new HttpListener();
                l.Prefixes.Add("http://127.0.0.1:0/");
                l.Start();
                _listener = l;
                _port = 52525;
            }

            ThreadPool.QueueUserWorkItem(ListenLoop);
        }

        private static void ListenLoop(object state)
        {
            while (_listener != null && _listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
                catch
                {
                    break;
                }
            }
        }

        private static void ProcessRequest(object state)
        {
            var ctx = (HttpListenerContext)state;
            try
            {
                var urlPath = ctx.Request.Url.AbsolutePath.TrimStart('/');
                if (string.IsNullOrEmpty(urlPath) || urlPath == "/")
                {
                    urlPath = "index.html";
                }

                byte[] data;
                if (!_cachedFiles.TryGetValue(urlPath, out data))
                {
                    // SPA Fallback: serve index.html for unknown routes
                    if (!_cachedFiles.TryGetValue("index.html", out data))
                    {
                        ctx.Response.StatusCode = 404;
                        ctx.Response.Close();
                        return;
                    }
                    urlPath = "index.html";
                }

                ctx.Response.ContentType = GetMimeType(urlPath);
                ctx.Response.ContentLength64 = data.Length;
                ctx.Response.Headers.Add("Cache-Control", "no-cache");
                ctx.Response.OutputStream.Write(data, 0, data.Length);
                ctx.Response.Close();
            }
            catch
            {
                try { ctx.Response.Abort(); } catch {}
            }
        }

        private static string GetMimeType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            switch (ext)
            {
                case ".html": return "text/html; charset=utf-8";
                case ".js": return "application/javascript; charset=utf-8";
                case ".css": return "text/css; charset=utf-8";
                case ".json": return "application/json; charset=utf-8";
                case ".webmanifest": return "application/manifest+json; charset=utf-8";
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".svg": return "image/svg+xml";
                case ".ico": return "image/x-icon";
                default: return "application/octet-stream";
            }
        }

        private static void LaunchAppWindow()
        {
            var url = "http://127.0.0.1:" + _port + "/";

            // Try launching Chrome or Edge in --app mode (looks like a standalone window!)
            string[] browserPaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Google\Chrome\Application\chrome.exe"),
            };

            foreach (var b in browserPaths)
            {
                if (File.Exists(b))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = b,
                            Arguments = "--app=\"" + url + "\"",
                            UseShellExecute = false
                        });
                        return;
                    }
                    catch {}
                }
            }

            // Fallback: Default browser
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch {}
        }

        private static void SetupTrayIcon()
        {
            var menu = new ContextMenu();
            menu.MenuItems.Add("Mở ứng dụng (Open App)", (s, e) => LaunchAppWindow());
            menu.MenuItems.Add("Sao chép liên kết (Copy URL)", (s, e) =>
            {
                Clipboard.SetText("http://127.0.0.1:" + _port + "/");
                _trayIcon.ShowBalloonTip(2000, "Đã sao chép", "Đã sao chép liên kết vào clipboard", ToolTipIcon.Info);
            });
            menu.MenuItems.Add("-");
            menu.MenuItems.Add("Thoát (Exit)", (s, e) =>
            {
                _trayIcon.Visible = false;
                Application.Exit();
            });

            _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "5D Lottery Simulator (Port " + _port + ")",
                ContextMenu = menu,
                Visible = true
            };

            _trayIcon.DoubleClick += (s, e) => LaunchAppWindow();
            _trayIcon.ShowBalloonTip(3000, "5D Lottery Simulator", "Ứng dụng đang chạy độc lập tại http://127.0.0.1:" + _port + "/", ToolTipIcon.Info);
        }

        private static void StopWebServer()
        {
            if (_listener != null)
            {
                try
                {
                    _listener.Stop();
                    _listener.Close();
                }
                catch {}
                _listener = null;
            }
        }
    }
}
