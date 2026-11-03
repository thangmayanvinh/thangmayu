# 5D LOTTERY SIMULATOR – HỆ THỐNG MÔ PHỎNG QUAY SỐ 5 LỒNG CẦU

Hệ thống phần mềm độc lập, chuyên nghiệp mô phỏng máy quay số 5 lồng cầu với hoạt ảnh 3D chân thực, cơ chế quay 27 lượt tự động với lượt thứ 27 màu đỏ nổi bật, hệ thống phân tích thống kê lịch sử chuyên sâu, thuật toán tạo số tham khảo với kỹ thuật làm trơn Laplace (Laplace smoothing), lưu trữ nội bộ an toàn (IndexedDB) và hỗ trợ PWA đầy đủ cho điện thoại Android, iPhone và máy tính.

---

## I. TỔNG QUAN VÀ NGUYÊN TẮC BẢO MẬT

1. **Bảo mật tuyệt đối (100% Offline-First / Client-Side)**:
   - Toàn bộ dữ liệu lịch sử và kết quả quay số được lưu trữ cục bộ trên thiết bị của người dùng thông qua **IndexedDB** (kèm cơ chế dự phòng LocalStorage).
   - **Không gửi bất kỳ dữ liệu nào ra bên ngoài máy chủ.**
   - Không sử dụng Firebase, Supabase, hoặc bất kỳ cơ sở dữ liệu đám mây nào của bên thứ ba.
   - Không chứa bất kỳ mã theo dõi, Google Analytics hoặc quảng cáo.
   - Hỗ trợ sao lưu toàn diện ra tệp JSON và phục hồi an toàn bất kỳ lúc nào.
   - Cho phép xóa sạch toàn bộ dữ liệu chỉ bằng một nút bấm.

2. **Khả năng tương thích & Triển khai PWA (4G/5G)**:
   - Hoạt động mượt mà trên Chrome, Edge, Safari, Firefox trên cả Máy tính, Android và iOS.
   - Giao diện đáp ứng (Responsive) tự động thích ứng với màn hình điện thoại và máy tính để bàn.
   - Có thể cài đặt trực tiếp thành ứng dụng PWA độc lập ngay trên màn hình chính của điện thoại.

---

## II. CƠ CHẾ HOẠT ĐỘNG CỦA 5 LỒNG CẦU

1. **Cấu tạo 5 lồng cầu**:
   - 5 lồng cầu tròn bằng hợp kim crom bạc, có các thanh kim loại bao quanh, vòng xích đạo và phễu thoát bóng phía dưới.
   - **Mỗi lồng chứa đúng 9 quả bóng tròn mang các số: 1, 2, 3, 4, 5, 6, 7, 8, 9 (Tuyệt đối không có số 0 trong lồng)**.
   - Quả bóng thiết kế 3D có độ bóng, đổ bóng, màu sắc phân biệt và số hiển thị sắc nét.
   - Mỗi lồng cầu tương ứng với 1 vị trí chữ số của kết quả 5 chữ số:
     - Lồng 1: Hàng chục nghìn
     - Lồng 2: Hàng nghìn
     - Lồng 3: Hàng trăm
     - Lồng 4: Hàng chục
     - Lồng 5: Hàng đơn vị

2. **Quy trình quay**:
   - Nhấn **QUAY 1 LƯỢT** hoặc **BẮT ĐẦU QUAY 27 LẦN**:
     - Cả 5 lồng cầu bắt đầu quay đồng thời với chuyển động vật lý hỗn loạn của 9 quả bóng bên trong.
     - Sau thời gian quay, bóng rơi qua phễu thoát xuống khay kiểm tra theo thứ tự lồng cầu.
     - Mỗi lồng chỉ xuất ra duy nhất 1 quả bóng.
     - Kết quả 5 chữ số được tổng hợp tức thì.
     - Các chữ số được phép trùng nhau giữa các lồng (ví dụ: `82519`, `31472`, `55555`).

3. **Chế độ quay 27 lần (Quan trọng nhất)**:
   - Hệ thống tự động thực hiện từ Lượt 01 đến Lượt 27.
   - Thanh tiến trình hiển thị tỷ lệ hoàn thành từ `1/27` đến `27/27`.
   - Các nút điều khiển linh hoạt: **Tạm dừng**, **Tiếp tục**, **Hủy / Làm mới**, điều chỉnh tốc độ quay (1x, 2x, 4x turbo).
   - **ĐẶC BIỆT KHI HOÀN THÀNH LƯỢT 27**:
     - Dừng toàn bộ chuyển động lồng cầu.
     - **TÔ MÀU ĐỎ RỰC RỠ CẢ 5 CHỮ SỐ CỦA LƯỢT CUỐI (LƯỢT 27)**.
     - Các kết quả từ lượt 01 đến 26 giữ màu hiển thị bình thường.
     - Phát nhạc chúc mừng hoàn thành (Fanfare) và hiệu ứng pháo hoa ăn mừng (Confetti).
     - Lưu kết quả phiên quay vào lịch sử và cho phép sao chép / xuất CSV.

---

## III. HỆ THỐNG PHÂN TÍCH THỐNG KÊ (STATISTICAL ANALYSIS)

Module phân tích chuyên sâu dữ liệu lịch sử do người dùng cung cấp (đã nạp sẵn 5 ngày mẫu):
- **Bảo toàn số 0 ở đầu chuỗi**: Tuyệt đối không chuyển sang số nguyên để tránh mất số 0 (ví dụ: `G.ĐB: 03917`, `G.4: 0487`, `0626`).
- **Phân tích tần suất**:
  - Tần suất các chữ số 0–9.
  - Tần suất chữ số theo từng vị trí lồng cầu (Hàng chục nghìn, Hàng nghìn, Hàng trăm, Hàng chục, Hàng đơn vị).
  - Xác định các số xuất hiện nhiều (Hot), số xuất hiện ít (Cold) và số chưa xuất hiện (Unseen).
- **Phân tích cấu trúc**:
  - Tỷ lệ chẵn / lẻ.
  - Tổng trung bình 5 chữ số.
  - Các cặp số xuất hiện liên tiếp nhiều nhất.
  - Các mẫu số lặp (kép: 00, 11, 22,...).
- **Lọc theo thời gian**: 1 ngày gần nhất, 3 ngày gần nhất, 5 ngày gần nhất, 7 ngày gần nhất hoặc toàn bộ dữ liệu.

---

## IV. THUẬT TOÁN TẠO SỐ NGẪU NHIÊN

Hệ thống cung cấp 2 chế độ:

1. **Chế độ 1: NGẪU NHIÊN ĐỘC LẬP**:
   - Sử dụng Cryptographic PRNG (`window.crypto.getRandomValues`) phân bố đều độc lập từ 1–9 cho từng lồng cầu.
   - Không bị ảnh hưởng bởi dữ liệu lịch sử.

2. **Chế độ 2: NGẪU NHIÊN CÓ THAM KHẢO THỐNG KÊ**:
   - Tích hợp trọng số tần suất theo vị trí và khoảng ngày được chọn.
   - **Làm trơn xác suất (Laplace smoothing)**: Đảm bảo mọi quả bóng từ 1 đến 9 luôn có xác suất xuất hiện $> 0$, không bao giờ bị loại bỏ hoàn toàn.
   - Thanh trượt điều chỉnh mức độ ảnh hưởng thống kê từ 0% đến 100%.
   - **Lưu ý khoa học**: Kết quả được tạo bằng phân bố xác suất toán học, không tuyên bố dự đoán trước kết quả xổ số thực tế.

---

## V. HƯỚNG DẪN CÀI ĐẶT VÀ SỬ DỤNG

### 1. Yêu cầu môi trường
- Node.js version 18+ (Đã cấu hình sẵn Node v20 LTS).

### 2. Cài đặt thư viện
```bash
npm install
```

### 3. Khởi chạy máy chủ phát triển
```bash
npm run dev
```
Trình duyệt sẽ mở tại: `http://localhost:5173`

### 4. Chạy kiểm thử tự động (15 bài test bắt buộc)
```bash
npm run test
```
Toàn bộ 15 yêu cầu kiểm thử và kiểm thử giao diện React Testing Library sẽ được chạy bằng **Vitest**.

### 5. Đóng gói bản Production
```bash
npm run build
```
Bản build tĩnh được tạo tại thư mục `dist/` cùng Service Worker PWA, sẵn sàng đưa lên bất kỳ máy chủ tĩnh HTTPS nào (Vercel, Cloudflare Pages, Netlify, Nginx, Apache).

---

## VI. CẤU TRÚC THƯ MỤC DỰ ÁN

```
bóng/
├── src/
│   ├── assets/
│   ├── common/
│   │   ├── SoundController.ts      # Bộ tổng hợp âm thanh Web Audio API (Offline 100%)
│   │   └── Header.tsx              # Thanh điều hướng và nút cài đặt PWA
│   ├── components/
│   │   ├── lottery/
│   │   │   ├── Ball3D.tsx          # Quả bóng 3D chi tiết với hiệu ứng ánh sáng
│   │   │   ├── Cage3DView.tsx      # Lồng cầu quay 3D Canvas 60 FPS, va chạm vật lý & phễu rơi
│   │   │   └── LotteryMachine.tsx  # Khung máy 5 lồng cầu, bảng điện tử, nút bấm điều khiển
│   │   ├── results/
│   │   │   └── DrawBoard27.tsx     # Bảng 27 lượt quay, lượt 27 màu đỏ rực rỡ, xuất CSV/sao chép
│   │   ├── statistics/
│   │   │   └── StatsDashboard.tsx  # Dashboard biểu đồ tần suất, 5 vị trí, chẵn/lẻ, cặp số
│   │   ├── history/
│   │   │   ├── HistoryManager.tsx  # Quản lý ngày lịch sử, nhập/dán văn bản, xuất JSON
│   │   │   └── HistorySeedData.ts  # Dữ liệu 5 ngày mẫu với số 0 ở đầu chuỗi
│   │   └── settings/
│   │       └── SettingsModal.tsx   # Cài đặt tốc độ, âm thanh, chế độ ngẫu nhiên, sao lưu/xóa
│   ├── engine/
│   │   ├── LotteryEngine.ts        # Máy trạng thái quay số 1 lượt và 27 lượt
│   │   ├── RandomGenerator.ts      # Bộ sinh số độc lập & tham khảo thống kê (Laplace smoothing)
│   │   └── StatisticsEngine.ts     # Bộ máy phân tích dữ liệu lịch sử
│   ├── storage/
│   │   └── db.ts                   # Cơ sở dữ liệu nội bộ IndexedDB & LocalStorage
│   ├── test/
│   │   ├── setup.ts                # Thiết lập mock Vitest, Web Audio API, Canvas
│   │   ├── LotterySimulator.test.ts# 15 bài kiểm thử bắt buộc theo yêu cầu đề bài
│   │   └── Components.test.tsx     # Kiểm thử giao diện và độ tương thích thiết bị
│   ├── types/
│   │   └── index.ts                # Định nghĩa kiểu dữ liệu TypeScript
│   ├── App.tsx                     # Ứng dụng chính kết nối 5 màn hình
│   ├── index.css                   # Phong cách Dark Luxury Metallic
│   └── main.tsx                    # Điểm khởi chạy React
├── public/
│   ├── favicon.svg
│   ├── pwa-192x192.png
│   └── pwa-512x512.png
├── index.html
├── vite.config.ts
├── tailwind.config.js
├── tsconfig.json
└── package.json
```
