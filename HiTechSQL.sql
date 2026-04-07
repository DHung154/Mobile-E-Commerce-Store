USE master;
GO

-- ==================================================================================
-- BƯỚC 1: DỌN DẸP DATABASE CŨ
-- ==================================================================================
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'HiTech')
BEGIN
    ALTER DATABASE HiTech SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HiTech;
END
GO

-- ==================================================================================
-- BƯỚC 2: TẠO DATABASE MỚI
-- ==================================================================================
CREATE DATABASE HiTech;
GO

USE HiTech;
GO

-- ==================================================================================
-- BƯỚC 3: TẠO CẤU TRÚC BẢNG
-- ==================================================================================

-- 1. NGƯỜI DÙNG
CREATE TABLE NguoiDung (
    MaND INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
);
GO

-- 2. TÀI KHOẢN
CREATE TABLE TaiKhoan (
    MaTK INT PRIMARY KEY IDENTITY(1,1),
    TenDangNhap NVARCHAR(100) UNIQUE NOT NULL,
    MatKhauHash VARCHAR(500) NOT NULL,
    VaiTro NVARCHAR(20) DEFAULT 'KhachHang',
    MaND INT UNIQUE NOT NULL,
    FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND)
);
GO

-- 3. DANH MỤC
CREATE TABLE DanhMuc (
    MaDM INT PRIMARY KEY IDENTITY(1,1),
    TenDM NVARCHAR(100) NOT NULL
);
GO

-- 4. LOẠI SẢN PHẨM
CREATE TABLE LoaiSP (
    MaLoai INT PRIMARY KEY IDENTITY(1,1),
    TenLoai NVARCHAR(100) NOT NULL,
    MaDM INT NOT NULL,
    FOREIGN KEY (MaDM) REFERENCES DanhMuc(MaDM)
);
GO

-- 5. THƯƠNG HIỆU
CREATE TABLE ThuongHieu (
    MaTH INT PRIMARY KEY IDENTITY(1,1),
    TenTH NVARCHAR(100),
    QuocGia NVARCHAR(100)
);
GO

-- 6. SẢN PHẨM
CREATE TABLE SanPham (
    MaSP INT PRIMARY KEY IDENTITY(1,1),
    TenSP NVARCHAR(200),
    GiamGia DECIMAL(18,2),
    GiaBan DECIMAL(18,2),
    MoTa NVARCHAR(MAX),
    HinhAnh NVARCHAR(255),
    SoLuongTon INT DEFAULT 0,
    MaDM INT,
    MaTH INT,
    MaLoai INT,
    NgaySanXuat DATETIME,
    HanSuDung DATETIME,
    RAM NVARCHAR(50),
    CPU NVARCHAR(50),
    Pin NVARCHAR(50),
    MauSac NVARCHAR(50),
    ThongSoKhac NVARCHAR(MAX),
    NgayNhapKho DATETIME DEFAULT GETDATE(),
    TrangThaiSanPham NVARCHAR(50) DEFAULT 'Kinh doanh',
    FOREIGN KEY (MaDM) REFERENCES DanhMuc(MaDM),
    FOREIGN KEY (MaTH) REFERENCES ThuongHieu(MaTH),
    FOREIGN KEY (MaLoai) REFERENCES LoaiSP(MaLoai)
);
GO

-- 7. ĐƠN HÀNG
CREATE TABLE DonHang (
    MaDH INT PRIMARY KEY IDENTITY(1,1),
    MaND INT,
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2),
    DiaChiGiaoHang NVARCHAR(255),
    TrangThaiThanhToan NVARCHAR(50) DEFAULT N'Chưa thanh toán',
    NgayThanhToan DATETIME NULL,
    PhuongThucThanhToan NVARCHAR(100) NULL,
    FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND)
);
GO

-- 8. CHI TIẾT ĐƠN HÀNG
CREATE TABLE ChiTietDonHang (
    MaDH INT,
    MaSP INT,
    SoLuong INT,
    DonGia DECIMAL(18,2),
    PRIMARY KEY (MaDH, MaSP),
    FOREIGN KEY (MaDH) REFERENCES DonHang(MaDH),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

-- 9. ĐÁNH GIÁ
CREATE TABLE DanhGia (
    MaDG INT PRIMARY KEY IDENTITY(1,1),
    MaSP INT,
    MaND INT,
    NoiDung NVARCHAR(500),
    Diem INT CHECK (Diem BETWEEN 1 AND 5),
    NgayDanhGia DATETIME DEFAULT GETDATE(),
    DuocApprove BIT DEFAULT 0,
    TraLoiAdmin NVARCHAR(500),
    ThoiGianTraLoi DATETIME,
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP),
    FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND)
);
GO

-- 10. THUỘC TÍNH & KHO
CREATE TABLE ThuocTinhSanPham (
    MaThuocTinh INT PRIMARY KEY IDENTITY(1,1),
    TenThuocTinh NVARCHAR(100) NOT NULL,
    LoaiThuocTinh NVARCHAR(50) NOT NULL,
    MoTa NVARCHAR(255)
);
GO

CREATE TABLE SanPham_ThuocTinh (
    MaSP INT,
    MaThuocTinh INT,
    PRIMARY KEY (MaSP, MaThuocTinh),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP) ON DELETE CASCADE,
    FOREIGN KEY (MaThuocTinh) REFERENCES ThuocTinhSanPham(MaThuocTinh)
);
GO

CREATE TABLE NhapHang (
    MaNhap INT PRIMARY KEY IDENTITY(1,1),
    MaSP INT NOT NULL,
    SoLuongNhap INT NOT NULL,
    NgayNhap DATETIME DEFAULT GETDATE(),
    NhaCungCap NVARCHAR(100),
    GiaVon DECIMAL(18,2),
    GhiChu NVARCHAR(255),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

CREATE TABLE XuatKho (
    MaXuat INT PRIMARY KEY IDENTITY(1,1),
    MaSP INT NOT NULL,
    SoLuongXuat INT NOT NULL,
    NgayXuat DATETIME DEFAULT GETDATE(),
    LyDoXuat NVARCHAR(50),
    GhiChu NVARCHAR(255),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

CREATE TABLE LienHe (
    MaLH INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100),
    Email NVARCHAR(100),
    SoDienThoai NVARCHAR(20),
    NoiDung NVARCHAR(1000),
    NgayGui DATETIME DEFAULT GETDATE()
);

-- ==================================================================================
-- BƯỚC 4: DỮ LIỆU MẪU CHO SHOP ĐIỆN THOẠI/LAPTOP
-- ==================================================================================

-- NGƯỜI DÙNG
INSERT INTO NguoiDung (HoTen, SoDienThoai, DiaChi, GioiTinh, NgaySinh)
VALUES
    (N'Nguyễn Văn An', '0912345678', N'Hà Nội', N'Nam', '1999-08-22'),
    (N'Lê Thị Thu', '0987654321', N'Đà Nẵng', N'Nữ', '2000-10-05'),
    (N'Trần Minh Tâm', '0933222111', N'Cần Thơ', N'Nam', '1998-12-30'),
    (N'Phạm Hồng Hoa', '0977334455', N'Nha Trang', N'Nữ', '2001-06-18');
GO

-- TÀI KHOẢN
INSERT INTO TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, MaND)
VALUES
    ('admin@hitech.vn', 'admin123', N'Admin', 1),
    ('an@gmail.com', '123456', N'KhachHang', 2),
    ('thu@gmail.com', '123456', N'KhachHang', 3),
    ('tam@gmail.com', '123456', N'KhachHang', 4);
GO

-- DANH MỤC
INSERT INTO DanhMuc (TenDM)
VALUES (N'Điện thoại'), (N'Laptop');
GO

-- LOẠI SẢN PHẨM
INSERT INTO LoaiSP (TenLoai, MaDM)
VALUES
    (N'Smartphone', 1),
    (N'Điện thoại phổ thông', 1),
    (N'Laptop văn phòng', 2),
    (N'Laptop gaming', 2);
GO

-- THƯƠNG HIỆU
INSERT INTO ThuongHieu (TenTH, QuocGia)
VALUES (N'Apple', N'Mỹ'), (N'Samsung', N'Hàn Quốc'), (N'Dell', N'Mỹ'), (N'HP', N'Mỹ'), (N'Lenovo', N'Trung Quốc');
GO

-- SẢN PHẨM
INSERT INTO SanPham (TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai, NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
    (N'iPhone 14', 0, 15000000, N'iPhone 14 chính hãng', 'iphone14.jpg', 50, 1, 1, 1, '2023-09-01', '2027-09-01', N'6GB', N'A15', N'3279mAh', N'Den', N'FaceID'),
    (N'Samsung Galaxy S23', 5, 20000000, N'Galaxy S23 chính hãng', 's23.jpg', 30, 1, 2, 1, '2023-02-15', '2026-02-15', N'8GB', N'Snapdragon 8 Gen2', N'3900mAh', N'Den', N'Chống nước'),
    (N'Dell XPS 13', 0, 32000000, N'Laptop Dell XPS 13', 'xps13.jpg', 20, 2, 3, 3, '2023-01-20', '2027-01-20', N'16GB', N'Intel i7', N'52Wh', N'Silver', N'SSD 512GB'),
    (N'HP Pavilion Gaming', 10, 28000000, N'Laptop HP Gaming', 'hp_gaming.jpg', 15, 2, 4, 4, '2023-03-01', '2027-03-01', N'16GB', N'Intel i7', N'70Wh', N'Den', N'RTX 3060'),
    (N'Lenovo ThinkPad X1', 0, 45000000, N'Laptop Lenovo X1', 'x1.jpg', 10, 2, 5, 3, '2023-04-10', '2027-04-10', N'32GB', N'Intel i9', N'80Wh', N'Den', N'SSD 1TB');
GO

-- ĐƠN HÀNG
INSERT INTO DonHang (MaND, TongTien, DiaChiGiaoHang)
VALUES
    (2, 15000000, N'Hà Nội'),
    (3, 28000000, N'Đà Nẵng'),
    (4, 32000000, N'Cần Thơ'),
    (2, 45000000, N'Hồ Chí Minh');
GO

-- CHI TIẾT ĐƠN HÀNG
INSERT INTO ChiTietDonHang (MaDH, MaSP, SoLuong, DonGia)
VALUES
    (1, 1, 1, 15000000),
    (2, 2, 1, 28000000),
    (3, 3, 1, 32000000),
    (4, 5, 1, 45000000);
GO

-- ĐÁNH GIÁ
INSERT INTO DanhGia (MaSP, MaND, NoiDung, Diem, DuocApprove)
VALUES
    (1, 2, N'Tuyệt vời, pin lâu!', 5, 1),
    (2, 3, N'Màn hình sắc nét.', 4, 1),
    (3, 4, N'Hiệu năng ổn.', 4, 1),
    (5, 2, N'Laptop mượt, nhẹ.', 5, 1);
GO

-- NHẬP HÀNG
INSERT INTO NhapHang (MaSP, SoLuongNhap, NgayNhap, NhaCungCap, GiaVon, GhiChu)
VALUES
    (1, 50, '2025-01-15', N'Apple VN', 12000000, N'Lô A'),
    (2, 30, '2025-02-01', N'Samsung VN', 20000000, N'Lô B'),
    (3, 20, '2025-01-20', N'Dell VN', 30000000, N'Lô C'),
    (4, 15, '2025-03-01', N'HP VN', 25000000, N'Lô D'),
    (5, 10, '2025-02-15', N'Lenovo VN', 40000000, N'Lô E');
GO

-- CẬP NHẬT TRẠNG THÁI TỒN KHO
UPDATE SanPham SET SoLuongTon = 50, TrangThaiSanPham = N'Còn hàng' WHERE MaSP = 1;
UPDATE SanPham SET SoLuongTon = 30, TrangThaiSanPham = N'Còn hàng' WHERE MaSP = 2;
UPDATE SanPham SET SoLuongTon = 20, TrangThaiSanPham = N'Còn hàng' WHERE MaSP = 3;
UPDATE SanPham SET SoLuongTon = 15, TrangThaiSanPham = N'Còn hàng' WHERE MaSP = 4;
UPDATE SanPham SET SoLuongTon = 10, TrangThaiSanPham = N'Còn hàng' WHERE MaSP = 5;
GO


INSERT INTO DanhMuc (TenDM)
VALUES (N'Phụ kiện');
GO

INSERT INTO LoaiSP (TenLoai, MaDM)
VALUES
    (N'Tai nghe', 3),
    (N'Sạc dự phòng', 3),
    (N'Cáp sạc', 3),
    (N'Ốp lưng', 3),
    (N'Chuột', 3),
    (N'Bàn phím', 3);
GO

INSERT INTO ThuongHieu (TenTH, QuocGia)
VALUES
    (N'Anker', N'Mỹ'),
    (N'Baseus', N'Trung Quốc'),
    (N'Logitech', N'Thụy Sĩ'),
    (N'Razer', N'Mỹ'),
    (N'Xiaomi', N'Trung Quốc');
GO

INSERT INTO SanPham (TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon,
                     MaDM, MaTH, MaLoai, NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
-- 6 - Tai nghe
(N'Tai nghe Bluetooth Anker Soundcore Q30', 5, 1500000,
 N'Tai nghe chống ồn chủ động ANC', 'anker_q30.jpg', 100,
 3, 1, 5, '2024-05-01', NULL, NULL, NULL, NULL, N'Den', N'ANC, Bluetooth 5.0'),

-- 7 - Sạc dự phòng
(N'Sạc dự phòng Baseus 20.000mAh', 0, 650000,
 N'Sạc nhanh PD 22.5W', 'baseus_powerbank.jpg', 150,
 3, 2, 6, '2024-06-01', NULL, NULL, NULL, NULL, N'Den', N'PD, QC 3.0'),

-- 8 - Cáp sạc
(N'Cáp sạc Type-C Xiaomi 1m', 0, 120000,
 N'Cáp sạc nhanh cho Android', 'xiaomi_cable.jpg', 200,
 3, 5, 7, '2024-01-10', NULL, NULL, NULL, NULL, N'Trang', N'2A'),

-- 9 - Ốp lưng
(N'Ốp lưng iPhone 14 trong suốt', 0, 90000,
 N'Ốp TPU dẻo trong suốt', 'op_iphone14.jpg', 80,
 3, NULL, 8, '2024-03-20', NULL, NULL, NULL, NULL, N'Trong suốt', N'TPU mềm'),

-- 10 - Chuột
(N'Chuột Logitech G102', 10, 450000,
 N'Chuột gaming LED RGB', 'g102.jpg', 70,
 3, 3, 9, '2024-02-15', NULL, NULL, NULL, NULL, N'Den', N'8000 DPI'),

-- 11 - Bàn phím
(N'Bàn phím Razer BlackWidow V3', 0, 2200000,
 N'Bàn phím cơ gaming switch xanh', 'razer_bw3.jpg', 40,
 3, 4, 10, '2024-04-01', NULL, NULL, NULL, NULL, N'Den', N'RGB Chroma');



INSERT INTO NhapHang (MaSP, SoLuongNhap, NhaCungCap, GiaVon, GhiChu)
VALUES
(6, 100, N'Anker VN', 1200000, N'Lô F'),
(7, 150, N'Baseus VN', 500000, N'Lô G'),
(8, 200, N'Xiaomi VN', 80000, N'Lô H'),
(9, 80, N'NCC Phụ Kiện', 60000, N'Lô I'),
(10, 70, N'Logitech VN', 350000, N'Lô J'),
(11, 40, N'Razer VN', 2000000, N'Lô K');
GO

UPDATE SanPham
SET TrangThaiSanPham = N'Còn hàng'
WHERE MaSP BETWEEN 6 AND 11;
GO

-- DANH MỤC MỚI
INSERT INTO DanhMuc (TenDM)
VALUES (N'Đồng hồ'), (N'TiVi');
GO


-- LOẠI SẢN PHẨM CHO ĐỒNG HỒ & TIVI
INSERT INTO LoaiSP (TenLoai, MaDM)
VALUES
    (N'Đồng hồ thông minh', 4),
    (N'Đồng hồ thời trang', 4),

    (N'TiVi QLED', 5),
    (N'TiVi OLED', 5);
GO


INSERT INTO ThuongHieu (TenTH, QuocGia)
VALUES
    (N'Casio', N'Nhật Bản'),
    (N'Sony', N'Nhật Bản'),
    (N'LG', N'Hàn Quốc');
GO


INSERT INTO SanPham (TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai, 
                     NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Casio MTP-1302', 0, 1200000, N'Đồng hồ thời trang nam', 'casio_mtp1302.jpg', 40,
 4, 11, 12, '2024-01-01', NULL, NULL, NULL, NULL, N'Bạc', N'Chống nước 50m'),

(N'Xiaomi Mi Band 8', 5, 900000, N'Vòng đeo tay thông minh', 'miband8.jpg', 80,
 4, 10, 11, '2024-02-01', NULL, N'2GB', N'Chip tiết kiệm năng lượng', N'200mAh', N'Đen', N'Cảm biến sức khỏe');
GO

INSERT INTO SanPham
(TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai,
 NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Sony Bravia 55 inch', 10, 18000000, N'Tivi Sony Bravia 4K', 'sony_bravia.jpg', 25,
 5, 12, 13, '2024-03-01', NULL, N'4GB', N'Google TV', NULL, N'Đen', N'HDR10+'),

(N'LG OLED C3 65 inch', 0, 38000000, N'Tivi LG OLED C3', 'lg_oled_c3.jpg', 15,
 5, 13, 14, '2024-03-10', NULL, N'6GB', N'α9 Gen6', NULL, N'Đen', N'4K 120Hz');
GO

INSERT INTO DanhMuc (TenDM)
VALUES (N'Âm thanh'), (N'Mic thu âm');
GO
INSERT INTO LoaiSP (TenLoai, MaDM)
VALUES
    (N'Loa Bluetooth', 6),
    (N'Tai nghe chụp tai', 6),
    (N'Tai nghe không dây', 6),

    (N'Micro phòng thu', 7),
    (N'Micro livestream', 7);
GO

INSERT INTO ThuongHieu (TenTH, QuocGia)
VALUES
    (N'JBL', N'Mỹ'),
    (N'Sony Audio', N'Nhật Bản'),
    (N'Audio Technica', N'Nhật Bản'),
    (N'Shure', N'Mỹ');
GO


INSERT INTO SanPham
(TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai,
 NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Loa JBL Charge 5', 5, 3600000, N'Loa Bluetooth JBL Charge 5 chống nước', 'jbl_charge5.jpg', 40,
 6, 14, 15, '2024-01-10', NULL, NULL, NULL, N'7500mAh', N'Đen', N'IP67, Bluetooth 5.1');
GO

INSERT INTO SanPham
(TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai,
 NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Sony WH-1000XM5', 0, 7500000, N'Tai nghe chụp tai chống ồn Sony', 'sony_xm5.jpg', 30,
 6, 15, 16, '2024-02-01', NULL, NULL, NULL, N'4000mAh', N'Đen', N'ANC, Bluetooth 5.2');
GO

INSERT INTO SanPham
(TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai,
 NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Apple AirPods Pro 2', 0, 5500000, N'Tai nghe không dây chống ồn Apple', 'airpods_pro2.jpg', 50,
 6, 1, 17, '2024-03-01', NULL, NULL, N'H2', NULL, N'Trắng', N'ANC, MagSafe');
GO

INSERT INTO SanPham
(TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai,
 NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Audio Technica AT2020', 0, 2500000, N'Micro phòng thu chất lượng cao', 'at2020.jpg', 20,
 7, 16, 18, '2024-01-20', NULL, NULL, NULL, NULL, N'Den', N'Condenser, Cardioid');
GO

INSERT INTO SanPham
(TenSP, GiamGia, GiaBan, MoTa, HinhAnh, SoLuongTon, MaDM, MaTH, MaLoai,
 NgaySanXuat, HanSuDung, RAM, CPU, Pin, MauSac, ThongSoKhac)
VALUES
(N'Shure MV7', 10, 5500000, N'Micro livestream nổi tiếng cho streamer', 'shure_mv7.jpg', 15,
 7, 17, 19, '2024-02-05', NULL, NULL, NULL, NULL, N'Bạc', N'USB/XLR, Voice Isolation');
GO


-- HOÀN TẤT
PRINT N'✅ Đã tạo Database HiTech với dữ liệu điện thoại/laptop thành công!';
GO
