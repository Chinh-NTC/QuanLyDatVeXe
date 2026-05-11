CREATE DATABASE QL_DatVeXe;
GO

USE QL_DatVeXe;
GO

/* =============================================================
   1. TINHTHANH — Danh mục tỉnh / thành phố
============================================================= */
CREATE TABLE TINHTHANH (
    maTinh   NVARCHAR(50)            PRIMARY KEY,
    tenTinh  NVARCHAR(100) NOT NULL UNIQUE,
    img      NVARCHAR(15)
);
GO

/* =============================================================
   2. PHUONGXA — Phường / xã thuộc tỉnh thành
============================================================= */
CREATE TABLE PHUONGXA (
    maPhuong  NVARCHAR(50)  PRIMARY KEY,
    maTinhNo  NVARCHAR(50)  NOT NULL,
    tenPhuong NVARCHAR(100) NOT NULL,

    CONSTRAINT FK_PHUONGXA_TINH
        FOREIGN KEY (maTinhNo) REFERENCES TINHTHANH(maTinh)
);
GO

/* =============================================================
   3. BENXE — Bến xe (điểm đi / điểm đến)
   [FIX] Xóa cột maPhuong thừa, đổi maPhuongNo sang NVARCHAR(50)
         để khớp kiểu với PHUONGXA.maPhuong
============================================================= */
CREATE TABLE BENXE (
    maBenXe    NVARCHAR(50)  PRIMARY KEY,
    maPhuongNo NVARCHAR(50)  NOT NULL,   -- [FIX] INT -> NVARCHAR(50)
    tenBenXe   NVARCHAR(150) NOT NULL,
    diaChi     NVARCHAR(255) NOT NULL,
    sdt        VARCHAR(15),

    CONSTRAINT FK_BENXE_PHUONG
        FOREIGN KEY (maPhuongNo) REFERENCES PHUONGXA(maPhuong)
);
GO

/* =============================================================
   4. TUYENDUONG — Tuyến đường giữa hai bến
   [FIX] thoiGianDuKien đổi từ NVARCHAR(50) sang INT (phút)
         để CHECK (> 0) có ý nghĩa
============================================================= */
CREATE TABLE TUYENDUONG (
    maTuyen        NVARCHAR(50)  PRIMARY KEY,
    maBenDi        NVARCHAR(50)  NOT NULL,
    maBenDen       NVARCHAR(50)  NOT NULL,
    khoangCach     DECIMAL(10,2) NOT NULL CHECK (khoangCach > 0),
    thoiGianDuKien INT           NOT NULL CHECK (thoiGianDuKien > 0), -- [FIX] đơn vị: phút

    CONSTRAINT FK_TUYEN_BENDI
        FOREIGN KEY (maBenDi)  REFERENCES BENXE(maBenXe),
    CONSTRAINT FK_TUYEN_BENDEN
        FOREIGN KEY (maBenDen) REFERENCES BENXE(maBenXe),
    CONSTRAINT CHK_BEN
        CHECK (maBenDi <> maBenDen),
    CONSTRAINT UQ_TUYENDUONG
        UNIQUE (maBenDi, maBenDen)
);
GO

/* =============================================================
   5. KHACHHANG
============================================================= */
CREATE TABLE KHACHHANG (
    maKH     NVARCHAR(50)  PRIMARY KEY,
	tenDangNhap VARCHAR(50)  NOT NULL UNIQUE,
    matKhau     VARCHAR(255) NOT NULL,
    hoTen    NVARCHAR(100) NOT NULL,
    sdt      VARCHAR(15)   NOT NULL UNIQUE,
    gioiTinh BIT,                             -- 1 nam, 0 nữ
    ngayTao  DATETIME      DEFAULT GETDATE()
);
GO

/* =============================================================
   6. NHANVIEN
   [FIX] maPhuongNo đổi từ INT sang NVARCHAR(50)
         để khớp kiểu với PHUONGXA.maPhuong
============================================================= */
CREATE TABLE NHANVIEN (
    maNV       NVARCHAR(50)  PRIMARY KEY,
	tenDangNhap VARCHAR(50)  NOT NULL UNIQUE,
    matKhau     VARCHAR(255) NOT NULL,
    hoTen      NVARCHAR(100) NOT NULL,
    sdt        VARCHAR(15)   UNIQUE,
    email      VARCHAR(100)  UNIQUE,
    diaChi     NVARCHAR(255),
    maPhuongNo NVARCHAR(50),              -- [FIX] INT -> NVARCHAR(50)
    chucVu     NVARCHAR(50),
    luong      DECIMAL(12,0) DEFAULT 0 CHECK (luong >= 0),
    ngayVaoLam DATE          DEFAULT GETDATE(),
    trangThai  NVARCHAR(20)  DEFAULT N'DANGLAM'
        CHECK (trangThai IN (N'DANGLAM', N'NGHIVIEC')),

    CONSTRAINT FK_NV_PX
        FOREIGN KEY (maPhuongNo) REFERENCES PHUONGXA(maPhuong)
);

GO

/* =============================================================
   8. NHAXE — Nhà xe / hãng vận tải
   [THÊM] maPhuongNo để liên kết địa chỉ với phường/xã
============================================================= */
CREATE TABLE NHAXE (
    maNhaXe    NVARCHAR(50)  PRIMARY KEY,
    tenNhaXe   NVARCHAR(150) NOT NULL,
    maPhuongNo NVARCHAR(50)  NULL,        -- [THÊM] FK -> PHUONGXA
    sdt        VARCHAR(15)   UNIQUE,
    email      VARCHAR(100),
    diaChi     NVARCHAR(255),

    CONSTRAINT FK_NHAXE_PHUONG
        FOREIGN KEY (maPhuongNo) REFERENCES PHUONGXA(maPhuong)
);
GO

/* =============================================================
   9. XE
============================================================= */
CREATE TABLE XE (
    bienSo    VARCHAR(15)  PRIMARY KEY,
    maNhaXe   NVARCHAR(50) NOT NULL,
    loaiXe    NVARCHAR(50),
    hangXe    NVARCHAR(50),
    namSX     INT          CHECK (namSX >= 2000),
    soTang    INT          DEFAULT 1 CHECK (soTang BETWEEN 1 AND 3),
    trangThai NVARCHAR(20) DEFAULT N'SANSANG'
        CHECK (trangThai IN (N'SANSANG', N'BAOTRI', N'DANGCHAY')),
    img       NVARCHAR(15),

    CONSTRAINT FK_XE_NHAXE
        FOREIGN KEY (maNhaXe) REFERENCES NHAXE(maNhaXe)
);
GO

/* =============================================================
   10. GHE — Ghế trên xe
============================================================= */
CREATE TABLE GHE (
    maGhe     NVARCHAR(50) PRIMARY KEY,
    bienSo    VARCHAR(15)  NOT NULL,
    soGhe     VARCHAR(10)  NOT NULL,
    tang      INT          DEFAULT 1,
    trangThai NVARCHAR(20) DEFAULT N'TRONG'
        CHECK (trangThai IN (N'TRONG', N'HONG', N'DADAT')),

    CONSTRAINT FK_GHE_XE
        FOREIGN KEY (bienSo) REFERENCES XE(bienSo),
    CONSTRAINT UQ_GHE
        UNIQUE (bienSo, soGhe)
);
GO

/* =============================================================
   11. CHUYENXE
============================================================= */
CREATE TABLE CHUYENXE (
    maChuyen  NVARCHAR(50)  PRIMARY KEY,
    maTuyen   NVARCHAR(50)  NOT NULL,
    bienSo    VARCHAR(15)   NOT NULL,
    ngayDi    DATE          NOT NULL,
    gioDi     TIME          NOT NULL,
    giaVe     DECIMAL(12,0) NOT NULL CHECK (giaVe > 0),
    trangThai NVARCHAR(20)  DEFAULT N'SAPDI'
        CHECK (trangThai IN (N'SAPDI', N'DANGTHANH', N'HOANTHANH', N'HOAN', N'HUY')),

    CONSTRAINT FK_CHUYENXE_TUYEN
        FOREIGN KEY (maTuyen) REFERENCES TUYENDUONG(maTuyen),
    CONSTRAINT FK_CHUYENXE_XE
        FOREIGN KEY (bienSo)  REFERENCES XE(bienSo)
);
GO

/* =============================================================
   12. DONDATVE
============================================================= */
CREATE TABLE DONDATVE (
    maDon     NVARCHAR(50)  PRIMARY KEY,
    maKH      NVARCHAR(50)  NOT NULL,
    maNV      NVARCHAR(50)  NULL,
    ngayDat   DATETIME      DEFAULT GETDATE(),
    tongTien  DECIMAL(12,0) DEFAULT 0 CHECK (tongTien >= 0),
    tienCoc   DECIMAL(12,0) DEFAULT 0 CHECK (tienCoc >= 0),
    trangThai NVARCHAR(20)  DEFAULT N'CHOXULY'
        CHECK (trangThai IN (N'CHOXULY', N'DAXACNHAN', N'DAHUY', N'HOANTHANH')),
    ghiChu    NVARCHAR(255),

    CONSTRAINT FK_DON_KH    FOREIGN KEY (maKH) REFERENCES KHACHHANG(maKH),
    CONSTRAINT FK_DON_NV    FOREIGN KEY (maNV) REFERENCES NHANVIEN(maNV),
    CONSTRAINT CHK_TIENCOC  CHECK (tienCoc <= tongTien)
);
GO

/* =============================================================
   13. CHITIETDATVE
============================================================= */
CREATE TABLE CHITIETDATVE (
    maCTDat     NVARCHAR(50)  PRIMARY KEY,
    maDon       NVARCHAR(50)  NOT NULL,
    maChuyen    NVARCHAR(50)  NOT NULL,
    maGhe       NVARCHAR(50)  NOT NULL,
    giaVeLucDat DECIMAL(12,0) NOT NULL CHECK (giaVeLucDat > 0),
    trangThaiVe NVARCHAR(20)  DEFAULT N'DADAT'
        CHECK (trangThaiVe IN (N'DADAT', N'DAHUY', N'DASUDUNG')),

    CONSTRAINT FK_CTDV_DON
        FOREIGN KEY (maDon)    REFERENCES DONDATVE(maDon) ON DELETE CASCADE,
    CONSTRAINT FK_CTDV_CHUYEN
        FOREIGN KEY (maChuyen) REFERENCES CHUYENXE(maChuyen),
    CONSTRAINT FK_CTDV_GHE
        FOREIGN KEY (maGhe)    REFERENCES GHE(maGhe),
    CONSTRAINT UQ_GHE_CHUYEN
        UNIQUE (maChuyen, maGhe)
);
GO

/* =============================================================
   14. THANHTOAN
============================================================= */
CREATE TABLE THANHTOAN (
    maTT       NVARCHAR(50)  PRIMARY KEY,
    maDon      NVARCHAR(50)  NOT NULL,
    soTien     DECIMAL(12,0) NOT NULL CHECK (soTien > 0),
    phuongThuc NVARCHAR(50)
        CHECK (phuongThuc IN (N'TIENMAT', N'CHUYENKHOAN')),
    thoiGianTT DATETIME      DEFAULT GETDATE(),
    trangThai  NVARCHAR(20)  DEFAULT N'THANHCONG'
        CHECK (trangThai IN (N'CHOXULY', N'THANHCONG', N'THATBAI')),

    CONSTRAINT FK_THANHTOAN_DON
        FOREIGN KEY (maDon) REFERENCES DONDATVE(maDon)
);
GO

/* =============================================================
   15. KHUYENMAI
============================================================= */
CREATE TABLE KHUYENMAI (
    maKM         NVARCHAR(50)  PRIMARY KEY,
    tenKhuyenMai NVARCHAR(150) NOT NULL,
    loaiKM       NVARCHAR(20)
        CHECK (loaiKM IN (N'PHANTRAM', N'CODINH')),
    giaTriGiam   DECIMAL(12,2) NOT NULL CHECK (giaTriGiam > 0),
    ngayBatDau   DATETIME      NOT NULL,
    ngayKetThuc  DATETIME      NOT NULL,

    CONSTRAINT CHK_KM_NGAY    CHECK (ngayKetThuc > ngayBatDau),
    CONSTRAINT CHK_KM_PHANTRAM CHECK (loaiKM <> N'PHANTRAM' OR giaTriGiam <= 100)
);
GO

/* =============================================================
   16. DONDATVE_KHUYENMAI
============================================================= */
CREATE TABLE DONDATVE_KHUYENMAI (
    maDon      NVARCHAR(50)  NOT NULL,
    maKM       NVARCHAR(50)  NOT NULL,
    soTienGiam DECIMAL(12,0) DEFAULT 0 CHECK (soTienGiam >= 0),

    PRIMARY KEY (maDon, maKM),

    CONSTRAINT FK_DDVKM_DON
        FOREIGN KEY (maDon) REFERENCES DONDATVE(maDon) ON DELETE CASCADE,
    CONSTRAINT FK_DDVKM_KM
        FOREIGN KEY (maKM)  REFERENCES KHUYENMAI(maKM)
);
GO

/* =============================================================
   17. DANHGIA
   [FIX] diemDanhGia đổi từ NVARCHAR(50) sang TINYINT
         để CHECK (BETWEEN 1 AND 5) hoạt động đúng
============================================================= */
CREATE TABLE DANHGIA (
    maDanhGia   NVARCHAR(50) PRIMARY KEY,
    maKH        NVARCHAR(50) NOT NULL,
    maChuyen    NVARCHAR(50) NOT NULL,
    diemDanhGia TINYINT      CHECK (diemDanhGia BETWEEN 1 AND 5), -- [FIX] NVARCHAR -> TINYINT
    binhLuan    NVARCHAR(500),
    ngayDanhGia DATETIME     DEFAULT GETDATE(),

    CONSTRAINT FK_DANHGIA_KH
        FOREIGN KEY (maKH)     REFERENCES KHACHHANG(maKH),
    CONSTRAINT FK_DANHGIA_CHUYEN
        FOREIGN KEY (maChuyen) REFERENCES CHUYENXE(maChuyen),
    CONSTRAINT UQ_DANHGIA_KH_CHUYEN
        UNIQUE (maKH, maChuyen)
);
GO

/* =============================================================
   INDEXES
============================================================= */
CREATE INDEX IX_CHUYENXE_NGAY    ON CHUYENXE(ngayDi);
CREATE INDEX IX_CHUYENXE_TRANG   ON CHUYENXE(trangThai);
CREATE INDEX IX_CTDV_CHUYEN      ON CHITIETDATVE(maChuyen);
CREATE INDEX IX_CTDV_TRANGTHAIDE ON CHITIETDATVE(trangThaiVe);
CREATE INDEX IX_DONDATVE_KH      ON DONDATVE(maKH);
CREATE INDEX IX_DANHGIA_CHUYEN   ON DANHGIA(maChuyen);
CREATE INDEX IX_THANHTOAN_DON    ON THANHTOAN(maDon);
GO

/* =============================================================
   VIEW: V_CHUYENXE
============================================================= */
CREATE VIEW V_CHUYENXE
AS
SELECT
    cx.maChuyen,
    cx.trangThai        AS trangThaiChuyen,
    bxdi.tenBenXe       AS benDi,
    bxden.tenBenXe      AS benDen,
    cx.ngayDi,
    cx.gioDi,
    cx.giaVe,
    x.loaiXe,
    nx.tenNhaXe,
    td.khoangCach,
    td.thoiGianDuKien,
    (SELECT COUNT(*) FROM GHE g
     WHERE g.bienSo = x.bienSo AND g.trangThai = N'TRONG') AS tongGheTot,
    (SELECT COUNT(*) FROM CHITIETDATVE ct
     WHERE ct.maChuyen = cx.maChuyen
       AND ct.trangThaiVe IN (N'DADAT', N'DASUDUNG')) AS soGheDaDat,
    (SELECT COUNT(*) FROM GHE g
     WHERE g.bienSo = x.bienSo AND g.trangThai = N'TRONG')
    -
    (SELECT COUNT(*) FROM CHITIETDATVE ct
     WHERE ct.maChuyen = cx.maChuyen
       AND ct.trangThaiVe IN (N'DADAT', N'DASUDUNG'))
    AS soGheTrong
FROM CHUYENXE cx
JOIN TUYENDUONG td ON cx.maTuyen  = td.maTuyen
JOIN BENXE bxdi    ON td.maBenDi  = bxdi.maBenXe
JOIN BENXE bxden   ON td.maBenDen = bxden.maBenXe
JOIN XE x          ON cx.bienSo   = x.bienSo
JOIN NHAXE nx      ON x.maNhaXe   = nx.maNhaXe
WHERE cx.trangThai <> N'HUY';
GO

/* =============================================================
   VIEW: V_DOANHTHU
============================================================= */
CREATE VIEW V_DOANHTHU
AS
SELECT
    YEAR(tt.thoiGianTT)  AS nam,
    MONTH(tt.thoiGianTT) AS thang,
    COUNT(DISTINCT tt.maDon) AS soDon,
    SUM(tt.soTien)           AS tongDoanhThu
FROM THANHTOAN tt
WHERE tt.trangThai = N'THANHCONG'
GROUP BY YEAR(tt.thoiGianTT), MONTH(tt.thoiGianTT);
GO

PRINT N'TAO DATABASE QL_DatVeXe THANH CONG';
GO
/* ==========================================================================================================================
   Các on update và on delete
========================================================================================================================== */
-- Bảo vệ Tỉnh thành: Không xóa Tỉnh nếu còn Phường xã
ALTER TABLE PHUONGXA DROP CONSTRAINT FK_PHUONGXA_TINH;
ALTER TABLE PHUONGXA ADD CONSTRAINT FK_PHUONGXA_TINH 
    FOREIGN KEY (maTinhNo) REFERENCES TINHTHANH(maTinh) ON UPDATE CASCADE ON DELETE NO ACTION;

-- Bảo vệ Phường xã: Không xóa Phường nếu còn Bến xe
ALTER TABLE BENXE DROP CONSTRAINT FK_BENXE_PHUONG;
ALTER TABLE BENXE ADD CONSTRAINT FK_BENXE_PHUONG 
    FOREIGN KEY (maPhuongNo) REFERENCES PHUONGXA(maPhuong) ON UPDATE CASCADE ON DELETE NO ACTION;

-- Bảo vệ Nhà xe: Không xóa Nhà xe nếu còn Xe
ALTER TABLE XE DROP CONSTRAINT FK_XE_NHAXE;
ALTER TABLE XE ADD CONSTRAINT FK_XE_NHAXE 
    FOREIGN KEY (maNhaXe) REFERENCES NHAXE(maNhaXe) ON UPDATE CASCADE ON DELETE NO ACTION;

-- Bảo vệ Xe: Không xóa Xe nếu đã có Chuyến xe được lập lịch
ALTER TABLE CHUYENXE DROP CONSTRAINT FK_CHUYENXE_XE;
ALTER TABLE CHUYENXE ADD CONSTRAINT FK_CHUYENXE_XE 
    FOREIGN KEY (bienSo) REFERENCES XE(bienSo) ON UPDATE CASCADE ON DELETE NO ACTION;

-- Bảo vệ Tuyến đường: Không xóa Tuyến nếu có Chuyến xe đang chạy
ALTER TABLE CHUYENXE DROP CONSTRAINT FK_CHUYENXE_TUYEN;
ALTER TABLE CHUYENXE ADD CONSTRAINT FK_CHUYENXE_TUYEN 
    FOREIGN KEY (maTuyen) REFERENCES TUYENDUONG(maTuyen) ON UPDATE CASCADE ON DELETE NO ACTION;

-- Bảo vệ Ghế & Chuyến: Không xóa Ghế/Chuyến nếu đã có vé bán ra (Lịch sử giao dịch)
ALTER TABLE CHITIETDATVE DROP CONSTRAINT FK_CTDV_CHUYEN;
ALTER TABLE CHITIETDATVE ADD CONSTRAINT FK_CTDV_CHUYEN 
    FOREIGN KEY (maChuyen) REFERENCES CHUYENXE(maChuyen) ON UPDATE CASCADE ON DELETE NO ACTION;

ALTER TABLE CHITIETDATVE DROP CONSTRAINT FK_CTDV_GHE;
ALTER TABLE CHITIETDATVE ADD CONSTRAINT FK_CTDV_GHE 
    FOREIGN KEY (maGhe) REFERENCES GHE(maGhe) ON UPDATE CASCADE ON DELETE NO ACTION;
-- Giữ đơn hàng khi xóa Nhân viên
ALTER TABLE DONDATVE DROP CONSTRAINT FK_DON_NV;
ALTER TABLE DONDATVE ADD CONSTRAINT FK_DON_NV 
    FOREIGN KEY (maNV) REFERENCES NHANVIEN(maNV) ON UPDATE CASCADE ON DELETE SET NULL;
-- Xóa Xe thì xóa Ghế (Vì Ghế là một phần của Xe)
-- Lưu ý: Lệnh này an toàn vì CHUYENXE đã chặn xóa Xe ở trên rồi.
ALTER TABLE GHE DROP CONSTRAINT FK_GHE_XE;
ALTER TABLE GHE ADD CONSTRAINT FK_GHE_XE 
    FOREIGN KEY (bienSo) REFERENCES XE(bienSo)  ON DELETE CASCADE;

-- Xóa Đơn đặt vé thì xóa Chi tiết đặt vé (Giải phóng dữ liệu đơn ảo)
ALTER TABLE CHITIETDATVE DROP CONSTRAINT FK_CTDV_DON;
ALTER TABLE CHITIETDATVE ADD CONSTRAINT FK_CTDV_DON 
    FOREIGN KEY (maDon) REFERENCES DONDATVE(maDon) ON UPDATE CASCADE ON DELETE CASCADE;

-- Xóa Đơn đặt vé thì xóa Khuyến mãi áp dụng cho đơn đó
ALTER TABLE DONDATVE_KHUYENMAI DROP CONSTRAINT FK_DDVKM_DON;
ALTER TABLE DONDATVE_KHUYENMAI ADD CONSTRAINT FK_DDVKM_DON 
    FOREIGN KEY (maDon) REFERENCES DONDATVE(maDon) ON UPDATE CASCADE ON DELETE CASCADE;
go
/* ==========================================================================================================================
   TRIGGER: Kiểm tra maGhe phải thuộc xe của chuyến
========================================================================================================================== */
CREATE OR ALTER TRIGGER TRG_CTDV_KIEMTRA_GHE_XE
ON CHITIETDATVE
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM INSERTED i
        JOIN CHUYENXE cx ON cx.maChuyen = i.maChuyen
        JOIN GHE g       ON g.maGhe     = i.maGhe
        WHERE g.bienSo <> cx.bienSo
    )
    BEGIN
        RAISERROR (N'Ghế không thuộc xe của chuyến này.', 16, 1);
        ROLLBACK TRANSACTION;
    END

    IF EXISTS (
        SELECT 1
        FROM INSERTED i
        JOIN GHE g ON g.maGhe = i.maGhe
        WHERE g.trangThai = N'HONG'
    )
    BEGIN
        RAISERROR (N'Ghế đang bị hỏng, không thể đặt.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

--============================================================================================================================================================
--============================================================================================================================================================
--============================================================================================================================================================




GO
-- 1. TINHTHANH (34 tỉnh thành sau sáp nhập kèm ảnh địa danh du lịch)
INSERT INTO TINHTHANH (maTinh, tenTinh, img) VALUES 
('T01', N'Cao Bằng', '32.jpg'), ('T02', N'Lào Cai', '14.jpg'), ('T03', N'Yên Bái', '2.jpg'),
('T04', N'Sơn La', '11.jpg'), ('T05', N'Hà Giang', '7.jpg'), ('T06', N'Lai Châu', '18.jpg'),
('T07', N'Hà Nội', '12.jpg'), ('T08', N'Nghệ An', '16.jpg'), ('T09', N'Nam Định', '25.jpg'),
('T10', N'Ninh Bình', '27.jpg'), ('T11', N'Phú Thọ', '31.webp'), ('T12', N'Quảng Bình', '28.jpg'),
('T13', N'Thừa Thiên Huế', '30.jpg'), ('T14', N'Quảng Trị', '13.jpg'), ('T15', N'Quảng Nam', '3.jpg'),
('T16', N'Quảng Ngãi', '4.jpg'), ('T17', N'Bình Định', '15.jpg'), ('T18', N'Đà Nẵng', '1.jpg'),
('T19', N'Kon Tum', '8.webp'), ('T20', N'Quảng Ninh', '23.jpg'), ('T21', N'Phú Yên', '26.jpg'),
('T22', N'Khánh Hòa', '29.jpg'), ('T23', N'Ninh Thuận', '19.jpg'), ('T24', N'Lâm Đồng', '24.png'),
('T25', N'Đắk Lắk', '21.jpg'), ('T26', N'TP. Hồ Chí Minh', '9.jpg'), ('T27', N'Bà Rịa - Vũng Tàu', '17.jpg'),
('T28', N'Bình Dương', '20.jpg'), ('T29', N'Bình Thuận', '6.jpg'), ('T30', N'Gia Lai', '22.jpg'),
('T31', N'Kiên Giang', '10.jpg'), ('T32', N'Cần Thơ', '5.jpg'), ('T33', N'Đồng Nai', '1.jpg'),
('T34', N'An Giang', '5.jpg');
GO

-- 2. PHUONGXA (68 phường/xã có thật - maTinhNo kiểu NVARCHAR(50))
INSERT INTO PHUONGXA (maPhuong, maTinhNo, tenPhuong) VALUES 
-- Miền Bắc
('PX01', 'T01', N'P. Sông Bằng'), ('PX02', 'T01', N'P. Hợp Giang'),
('PX03', 'T02', N'P. Lào Cai'), ('PX04', 'T02', N'P. Sa Pa'),
('PX05', 'T03', N'P. Đồng Tâm'), ('PX06', 'T03', N'P. Minh Tân'),
('PX07', 'T04', N'P. Chiềng Lề'), ('PX08', 'T04', N'P. Quyết Thắng'),
('PX09', 'T05', N'P. Minh Khai'), ('PX10', 'T05', N'P. Trần Phú'),
('PX11', 'T06', N'P. Quyết Tiến'), ('PX12', 'T06', N'P. Tân Phong'),
('PX13', 'T07', N'P. Hàng Bạc'), ('PX14', 'T07', N'P. Mỹ Đình 1'),
('PX15', 'T08', N'P. Quang Trung'), ('PX16', 'T08', N'P. Lê Lợi'),
('PX17', 'T09', N'P. Vị Xuyên'), ('PX18', 'T09', N'P. Năng Tĩnh'),
('PX19', 'T10', N'P. Vân Giang'), ('PX20', 'T10', N'P. Thanh Bình'),
('PX21', 'T11', N'P. Gia Cẩm'), ('PX22', 'T11', N'P. Tiên Cát'),
('PX23', 'T12', N'P. Đồng Hải'), ('PX24', 'T12', N'P. Hải Đình'),
-- Miền Trung
('PX25', 'T13', N'P. Phú Hội'), ('PX26', 'T13', N'P. Vĩnh Ninh'),
('PX27', 'T14', N'P. 1'), ('PX28', 'T14', N'P. Đông Lương'),
('PX29', 'T15', N'P. Tân Thạnh'), ('PX30', 'T15', N'P. An Mỹ'),
('PX31', 'T16', N'P. Quảng Phú'), ('PX32', 'T16', N'P. Nghĩa Lộ'),
('PX33', 'T17', N'P. Lê Hồng Phong'), ('PX34', 'T17', N'P. Nguyễn Văn Cừ'),
('PX35', 'T18', N'P. Thạch Thang'), ('PX36', 'T18', N'P. Hòa Xuân'),
('PX37', 'T19', N'P. Quang Trung'), ('PX38', 'T19', N'P. Quyết Thắng'),
('PX39', 'T20', N'P. Bạch Đằng'), ('PX40', 'T20', N'P. Hồng Gai'),
('PX41', 'T21', N'P. 1'), ('PX42', 'T21', N'P. 9'),
('PX43', 'T22', N'P. Lộc Thọ'), ('PX44', 'T22', N'P. Vĩnh Nguyên'),
('PX45', 'T23', N'P. Mỹ Hải'), ('PX46', 'T23', N'P. Kinh Dinh'),
-- Tây Nguyên & Miền Nam
('PX47', 'T24', N'P. 1'), ('PX48', 'T24', N'P. 10'),
('PX49', 'T25', N'P. Tân Lập'), ('PX50', 'T25', N'P. Thắng Lợi'),
('PX51', 'T26', N'P. Bến Nghé'), ('PX52', 'T26', N'P. Tân Định'),
('PX53', 'T27', N'P. Thắng Tam'), ('PX54', 'T27', N'P. Nguyễn An Ninh'),
('PX55', 'T28', N'P. Phú Cường'), ('PX56', 'T28', N'P. Hiệp Thành'),
('PX57', 'T29', N'P. Mũi Né'), ('PX58', 'T29', N'P. Đức Thắng'),
('PX59', 'T30', N'P. Tây Sơn'), ('PX60', 'T30', N'P. Hoa Lư'),
('PX61', 'T31', N'P. Vĩnh Thanh Vân'), ('PX62', 'T31', N'P. An Hòa'),
('PX63', 'T32', N'P. Tân An'), ('PX64', 'T32', N'P. Cái Khế'),
('PX65', 'T33', N'P. Thanh Bình'), ('PX66', 'T33', N'P. Quyết Thắng'),
('PX67', 'T34', N'P. Mỹ Long'), ('PX68', 'T34', N'P. Mỹ Bình');
GO
-- 8. NHAXE (Địa chỉ chi tiết gắn với mã phường xã thật đã nạp ở Phần 1)
INSERT INTO NHAXE (maNhaXe, tenNhaXe, maPhuongNo, sdt, email, diaChi) VALUES 
('NX01', N'Phương Trang (FUTA Bus)', 'PX47', '19006067', 'hotro@futa.vn', N'Số 01 Tô Hiến Thành'),
('NX02', N'Thành Bưởi', 'PX51', '19006079', 'chamsockh@thanhbuoi.com.vn', N'Số 266-268 Lê Hồng Phong'),
('NX03', N'Văn Minh', 'PX15', '19001231', 'vanminh@gmail.com', N'Số 168 Phan Chu Trinh'),
('NX04', N'Quỳnh Thanh Limo', 'PX14', '19002234', 'quynhthanh@gmail.com', N'Lô 21, Khu đô thị Mỹ Đình 1'),
('NX05', N'Tiến Oanh', 'PX49', '19006084', 'tienoanh@gmail.com', N'Số 134 Hai Bà Trưng'),
('NX06', N'Sao Việt', 'PX03', '19006746', 'saoviet@gmail.com', N'Số 07 Phạm Văn Đồng'),
('NX07', N'Cúc Tùng', 'PX43', '19006606', 'cuctung@gmail.com', N'Số 198 Ngô Gia Tự'),
('NX08', N'Thuận Thảo', 'PX41', '19002255', 'thuanthao@gmail.com', N'Số 227 Nguyễn Tất Thành');
GO

INSERT INTO KHACHHANG (maKH, tenDangNhap, matKhau, hoTen, sdt, gioiTinh, ngayTao) VALUES 
('KH01', 'an_nguyen', 'pass123', N'Nguyễn Văn An', '0901234567', 1, GETDATE()),
('KH02', 'binh_tran', 'pass123', N'Trần Thị Bình', '0912345678', 0, GETDATE()),
('KH03', 'cuong_le', 'pass123', N'Lê Quang Cường', '0923456789', 1, GETDATE()),
('KH04', 'duc_pham', 'pass123', N'Phạm Minh Đức', '0934567890', 1, GETDATE()),
('KH05', 'lan_anh', 'pass123', N'Hoàng Lan Anh', '0945678901', 0, GETDATE()),
('KH06', 'hung_dung', 'pass123', N'Đỗ Hùng Dũng', '0956789012', 1, GETDATE()),
('KH07', 'tuyet_mai', 'pass123', N'Vũ Tuyết Mai', '0967890123', 0, GETDATE());
go

/* =============================================================
   6. NHANVIEN (Sửa maPhuong thành maPhuongNo NVARCHAR(50))
============================================================= */
INSERT INTO NHANVIEN (maNV, tenDangNhap, matKhau, hoTen, sdt, email, diaChi, maPhuongNo, chucVu, luong, ngayVaoLam, trangThai) VALUES 
('NV01', 'tung_nv', 'staff1', N'Lý Thanh Tùng', '0801112222', 'tungly@gmail.com', N'Số 12 Phan Chu Trinh', 'PX13', N'Bán vé', 8000000, '2025-10-15', N'DANGLAM'),
('NV02', 'linh_nv', 'staff2', N'Nguyễn Mỹ Linh', '0803334444', 'linhnm@gmail.com', N'Số 88 Lê Hồng Phong', 'PX51', N'Bán vé', 10000000, '2020-11-20', N'DANGLAM'),
('NV03', 'tam_nv', 'staff3', N'Bùi Minh Tâm', '0812223333', 'tambm@gmail.com', N'Số 45 Tân Lập', 'PX49', N'Bán vé', 8000000, '2026-03-05', N'DANGLAM');



GO


-- 9. XE (Mỗi hàng là 1 ảnh, xác định loại xe và số tầng theo ảnh nội/ngoại thất)
INSERT INTO XE (bienSo, maNhaXe, loaiXe, hangXe, namSX, soTang, trangThai, img) VALUES 
-- Nhóm xe Giường nằm / Phòng nằm (2 tầng)
('49F-005.76', 'NX01', N'Giường nằm Mobihome', 'Thaco', 2022, 2, N'SANSANG', 'x12.jpg'),
('38B-012.42', 'NX03', N'Giường nằm 40 chỗ', 'Thaco', 2021, 2, N'SANSANG', 'x6.jpg'),
('50E-802.84', 'NX02', N'Limousine 22 Phòng', 'Thaco', 2023, 2, N'SANSANG', 'x14.jpg'),
('51B-115.00', 'NX02', N'Cung điện di động', 'Hyundai', 2023, 2, N'SANSANG', '15.jpg'),
('53S-8471',   'NX01', N'Giường nằm tiêu chuẩn', 'Hyundai', 2021, 2, N'SANSANG', 'x7.jpg'),
('60B-034.21', 'NX01', N'Sleeper Bus High-class', 'Thaco', 2022, 2, N'SANSANG', 'x23.jpg'),
('51B-296.17', 'NX02', N'Giường nằm VIP', 'Thaco', 2023, 2, N'SANSANG', 'x19.jpg'),
('XE-INT-01',  'NX01', N'Nội thất Giường nằm đỏ', 'Thaco', 2022, 2, N'SANSANG', 'x1.jpg'),
('XE-INT-02',  'NX02', N'Nội thất Giường nằm cam', 'Thaco', 2022, 2, N'SANSANG', 'x5.jpg'),
('XE-INT-03',  'NX06', N'Cabin Cung điện Blue-LED', 'Thaco', 2024, 2, N'SANSANG', 'x16.webp'),

-- Nhóm xe Limousine Ghế ngồi (1 tầng)
('29B-555.99', 'NX04', N'Limousine VIP 9 chỗ', 'Ford', 2022, 1, N'SANSANG', 'x9.webp'),
('49F-005.75', 'NX05', N'Limousine 12 chỗ', 'Solati', 2023, 1, N'SANSANG', 'x10.jpg'),
('15B-999.01', 'NX06', N'Dcar Limousine 11 chỗ', 'Dcar', 2023, 1, N'SANSANG', 'x11.webp'),
('47B-111.22', 'NX05', N'Limousine Thương gia', 'Solati', 2023, 1, N'SANSANG', 'x21.webp'),
('30F-999.13', 'NX04', N'Limousine Business', 'Dcar', 2023, 1, N'SANSANG', 'x13.jpg'),
('18B-007.89', 'NX08', N'Limousine Purple Night', 'Universe', 2022, 1, N'SANSANG', 'x22.jpg'),
('29B-888.18', 'NX04', N'Limousine 19 chỗ', 'Fuso', 2021, 1, N'SANSANG', 'x20.jpg'),

-- Nhóm xe Khách Ghế ngồi (1 tầng)
('72B-004.56', 'NX02', N'Ghế ngồi 45 chỗ', 'Samco', 2020, 1, N'SANSANG', 'x3.jpg'),
('50B-111.04', 'NX03', N'Ghế ngồi 47 chỗ', 'Daewoo', 2021, 1, N'SANSANG', 'x4.jpg'),
('17B-002.11', 'NX04', N'Ghế ngồi 29 chỗ', 'Samco', 2021, 1, N'SANSANG', 'x8.jpg'),
('43B-002.77', 'NX01', N'Ghế ngồi 35 chỗ', 'Thaco', 2022, 1, N'SANSANG', 'x2.jpg'),
('65B-003.44', 'NX03', N'Ghế ngồi 45 chỗ', 'Hyundai', 2020, 1, N'SANSANG', 'x17.jpg'),
('16B-637.74', 'NX06', N'Hyundai Global 29 chỗ', 'Hyundai', 2022, 1, N'SANSANG', 'x18.jpg');
GO


-- 1. Xe '49F-005.76' (Giường nằm Mobihome - 40 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G001', '49F-005.76', 'A01', 1, N'TRONG'), ('G002', '49F-005.76', 'A02', 1, N'TRONG'), ('G003', '49F-005.76', 'A03', 1, N'TRONG'), ('G004', '49F-005.76', 'A04', 1, N'TRONG'), ('G005', '49F-005.76', 'A05', 1, N'TRONG'),
('G006', '49F-005.76', 'A06', 1, N'TRONG'), ('G007', '49F-005.76', 'A07', 1, N'TRONG'), ('G008', '49F-005.76', 'A08', 1, N'TRONG'), ('G009', '49F-005.76', 'A09', 1, N'TRONG'), ('G010', '49F-005.76', 'A10', 1, N'TRONG'),
('G011', '49F-005.76', 'A11', 1, N'TRONG'), ('G012', '49F-005.76', 'A12', 1, N'TRONG'), ('G013', '49F-005.76', 'A13', 1, N'TRONG'), ('G014', '49F-005.76', 'A14', 1, N'TRONG'), ('G015', '49F-005.76', 'A15', 1, N'TRONG'),
('G016', '49F-005.76', 'A16', 1, N'TRONG'), ('G017', '49F-005.76', 'A17', 1, N'TRONG'), ('G018', '49F-005.76', 'A18', 1, N'TRONG'), ('G019', '49F-005.76', 'A19', 1, N'TRONG'), ('G020', '49F-005.76', 'A20', 1, N'TRONG'),
('G021', '49F-005.76', 'B01', 2, N'TRONG'), ('G022', '49F-005.76', 'B02', 2, N'TRONG'), ('G023', '49F-005.76', 'B03', 2, N'TRONG'), ('G024', '49F-005.76', 'B04', 2, N'TRONG'), ('G025', '49F-005.76', 'B05', 2, N'TRONG'),
('G026', '49F-005.76', 'B06', 2, N'TRONG'), ('G027', '49F-005.76', 'B07', 2, N'TRONG'), ('G028', '49F-005.76', 'B08', 2, N'TRONG'), ('G029', '49F-005.76', 'B09', 2, N'TRONG'), ('G030', '49F-005.76', 'B10', 2, N'TRONG'),
('G031', '49F-005.76', 'B11', 2, N'TRONG'), ('G032', '49F-005.76', 'B12', 2, N'TRONG'), ('G033', '49F-005.76', 'B13', 2, N'TRONG'), ('G034', '49F-005.76', 'B14', 2, N'TRONG'), ('G035', '49F-005.76', 'B15', 2, N'TRONG'),
('G036', '49F-005.76', 'B16', 2, N'TRONG'), ('G037', '49F-005.76', 'B17', 2, N'TRONG'), ('G038', '49F-005.76', 'B18', 2, N'TRONG'), ('G039', '49F-005.76', 'B19', 2, N'TRONG'), ('G040', '49F-005.76', 'B20', 2, N'TRONG');

-- 2. Xe '38B-012.42' (Giường nằm Thaco vàng - 40 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G041', '38B-012.42', 'A01', 1, N'TRONG'), ('G042', '38B-012.42', 'A02', 1, N'TRONG'), ('G043', '38B-012.42', 'A03', 1, N'TRONG'), ('G044', '38B-012.42', 'A04', 1, N'TRONG'), ('G045', '38B-012.42', 'A05', 1, N'TRONG'),
('G046', '38B-012.42', 'A06', 1, N'TRONG'), ('G047', '38B-012.42', 'A07', 1, N'TRONG'), ('G048', '38B-012.42', 'A08', 1, N'TRONG'), ('G049', '38B-012.42', 'A09', 1, N'TRONG'), ('G050', '38B-012.42', 'A10', 1, N'TRONG'),
('G051', '38B-012.42', 'A11', 1, N'TRONG'), ('G052', '38B-012.42', 'A12', 1, N'TRONG'), ('G053', '38B-012.42', 'A13', 1, N'TRONG'), ('G054', '38B-012.42', 'A14', 1, N'TRONG'), ('G055', '38B-012.42', 'A15', 1, N'TRONG'),
('G056', '38B-012.42', 'A16', 1, N'TRONG'), ('G057', '38B-012.42', 'A17', 1, N'TRONG'), ('G058', '38B-012.42', 'A18', 1, N'TRONG'), ('G059', '38B-012.42', 'A19', 1, N'TRONG'), ('G060', '38B-012.42', 'A20', 1, N'TRONG'),
('G061', '38B-012.42', 'B01', 2, N'TRONG'), ('G062', '38B-012.42', 'B02', 2, N'TRONG'), ('G063', '38B-012.42', 'B03', 2, N'TRONG'), ('G064', '38B-012.42', 'B04', 2, N'TRONG'), ('G065', '38B-012.42', 'B05', 2, N'TRONG'),
('G066', '38B-012.42', 'B06', 2, N'TRONG'), ('G067', '38B-012.42', 'B07', 2, N'TRONG'), ('G068', '38B-012.42', 'B08', 2, N'TRONG'), ('G069', '38B-012.42', 'B09', 2, N'TRONG'), ('G070', '38B-012.42', 'B10', 2, N'TRONG'),
('G071', '38B-012.42', 'B11', 2, N'TRONG'), ('G072', '38B-012.42', 'B12', 2, N'TRONG'), ('G073', '38B-012.42', 'B13', 2, N'TRONG'), ('G074', '38B-012.42', 'B14', 2, N'TRONG'), ('G075', '38B-012.42', 'B15', 2, N'TRONG'),
('G076', '38B-012.42', 'B16', 2, N'TRONG'), ('G077', '38B-012.42', 'B17', 2, N'TRONG'), ('G078', '38B-012.42', 'B18', 2, N'TRONG'), ('G079', '38B-012.42', 'B19', 2, N'TRONG'), ('G080', '38B-012.42', 'B20', 2, N'TRONG');

-- 3. Xe '50E-802.84' (Limousine 22 Phòng - 22 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G081', '50E-802.84', 'P01', 1, N'TRONG'), ('G082', '50E-802.84', 'P02', 1, N'TRONG'), ('G083', '50E-802.84', 'P03', 1, N'TRONG'), ('G084', '50E-802.84', 'P04', 1, N'TRONG'), ('G085', '50E-802.84', 'P05', 1, N'TRONG'), ('G086', '50E-802.84', 'P06', 1, N'TRONG'), ('G087', '50E-802.84', 'P07', 1, N'TRONG'), ('G088', '50E-802.84', 'P08', 1, N'TRONG'), ('G089', '50E-802.84', 'P09', 1, N'TRONG'), ('G090', '50E-802.84', 'P10', 1, N'TRONG'), ('G091', '50E-802.84', 'P11', 1, N'TRONG'),
('G092', '50E-802.84', 'P12', 2, N'TRONG'), ('G093', '50E-802.84', 'P13', 2, N'TRONG'), ('G094', '50E-802.84', 'P14', 2, N'TRONG'), ('G095', '50E-802.84', 'P15', 2, N'TRONG'), ('G096', '50E-802.84', 'P16', 2, N'TRONG'), ('G097', '50E-802.84', 'P17', 2, N'TRONG'), ('G098', '50E-802.84', 'P18', 2, N'TRONG'), ('G099', '50E-802.84', 'P19', 2, N'TRONG'), ('G100', '50E-802.84', 'P20', 2, N'TRONG'), ('G101', '50E-802.84', 'P21', 2, N'TRONG'), ('G102', '50E-802.84', 'P22', 2, N'TRONG');

-- 4. Xe '51B-115.00' (Cung điện di động - 22 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G103', '51B-115.00', 'P01', 1, N'TRONG'), ('G104', '51B-115.00', 'P02', 1, N'TRONG'), ('G105', '51B-115.00', 'P03', 1, N'TRONG'), ('G106', '51B-115.00', 'P04', 1, N'TRONG'), ('G107', '51B-115.00', 'P05', 1, N'TRONG'), ('G108', '51B-115.00', 'P06', 1, N'TRONG'), ('G109', '51B-115.00', 'P07', 1, N'TRONG'), ('G110', '51B-115.00', 'P08', 1, N'TRONG'), ('G111', '51B-115.00', 'P09', 1, N'TRONG'), ('G112', '51B-115.00', 'P10', 1, N'TRONG'), ('G113', '51B-115.00', 'P11', 1, N'TRONG'),
('G114', '51B-115.00', 'P12', 2, N'TRONG'), ('G115', '51B-115.00', 'P13', 2, N'TRONG'), ('G116', '51B-115.00', 'P14', 2, N'TRONG'), ('G117', '51B-115.00', 'P15', 2, N'TRONG'), ('G118', '51B-115.00', 'P16', 2, N'TRONG'), ('G119', '51B-115.00', 'P17', 2, N'TRONG'), ('G120', '51B-115.00', 'P18', 2, N'TRONG'), ('G121', '51B-115.00', 'P19', 2, N'TRONG'), ('G122', '51B-115.00', 'P20', 2, N'TRONG'), ('G123', '51B-115.00', 'P21', 2, N'TRONG'), ('G124', '51B-115.00', 'P22', 2, N'TRONG');

-- 5. Xe '53S-8471' (Giường nằm Phương Trang - 40 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G125', '53S-8471', 'A01', 1, N'TRONG'), ('G126', '53S-8471', 'A02', 1, N'TRONG'), ('G127', '53S-8471', 'A03', 1, N'TRONG'), ('G128', '53S-8471', 'A04', 1, N'TRONG'), ('G129', '53S-8471', 'A05', 1, N'TRONG'), ('G130', '53S-8471', 'A06', 1, N'TRONG'), ('G131', '53S-8471', 'A07', 1, N'TRONG'), ('G132', '53S-8471', 'A08', 1, N'TRONG'), ('G133', '53S-8471', 'A09', 1, N'TRONG'), ('G134', '53S-8471', 'A10', 1, N'TRONG'), ('G135', '53S-8471', 'A11', 1, N'TRONG'), ('G136', '53S-8471', 'A12', 1, N'TRONG'), ('G137', '53S-8471', 'A13', 1, N'TRONG'), ('G138', '53S-8471', 'A14', 1, N'TRONG'), ('G139', '53S-8471', 'A15', 1, N'TRONG'), ('G140', '53S-8471', 'A16', 1, N'TRONG'), ('G141', '53S-8471', 'A17', 1, N'TRONG'), ('G142', '53S-8471', 'A18', 1, N'TRONG'), ('G143', '53S-8471', 'A19', 1, N'TRONG'), ('G144', '53S-8471', 'A20', 1, N'TRONG'),
('G145', '53S-8471', 'B01', 2, N'TRONG'), ('G146', '53S-8471', 'B02', 2, N'TRONG'), ('G147', '53S-8471', 'B03', 2, N'TRONG'), ('G148', '53S-8471', 'B04', 2, N'TRONG'), ('G149', '53S-8471', 'B05', 2, N'TRONG'), ('G150', '53S-8471', 'B06', 2, N'TRONG'), ('G151', '53S-8471', 'B07', 2, N'TRONG'), ('G152', '53S-8471', 'B08', 2, N'TRONG'), ('G153', '53S-8471', 'B09', 2, N'TRONG'), ('G154', '53S-8471', 'B10', 2, N'TRONG'), ('G155', '53S-8471', 'B11', 2, N'TRONG'), ('G156', '53S-8471', 'B12', 2, N'TRONG'), ('G157', '53S-8471', 'B13', 2, N'TRONG'), ('G158', '53S-8471', 'B14', 2, N'TRONG'), ('G159', '53S-8471', 'B15', 2, N'TRONG'), ('G160', '53S-8471', 'B16', 2, N'TRONG'), ('G161', '53S-8471', 'B17', 2, N'TRONG'), ('G162', '53S-8471', 'B18', 2, N'TRONG'), ('G163', '53S-8471', 'B19', 2, N'TRONG'), ('G164', '53S-8471', 'B20', 2, N'TRONG');

-- 6. Xe '60B-034.21' (Sleeper Bus High-class - 34 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G165', '60B-034.21', 'A01', 1, N'TRONG'), ('G166', '60B-034.21', 'A02', 1, N'TRONG'), ('G167', '60B-034.21', 'A03', 1, N'TRONG'), ('G168', '60B-034.21', 'A04', 1, N'TRONG'), ('G169', '60B-034.21', 'A05', 1, N'TRONG'), ('G170', '60B-034.21', 'A06', 1, N'TRONG'), ('G171', '60B-034.21', 'A07', 1, N'TRONG'), ('G172', '60B-034.21', 'A08', 1, N'TRONG'), ('G173', '60B-034.21', 'A09', 1, N'TRONG'), ('G174', '60B-034.21', 'A10', 1, N'TRONG'), ('G175', '60B-034.21', 'A11', 1, N'TRONG'), ('G176', '60B-034.21', 'A12', 1, N'TRONG'), ('G177', '60B-034.21', 'A13', 1, N'TRONG'), ('G178', '60B-034.21', 'A14', 1, N'TRONG'), ('G179', '60B-034.21', 'A15', 1, N'TRONG'), ('G180', '60B-034.21', 'A16', 1, N'TRONG'), ('G181', '60B-034.21', 'A17', 1, N'TRONG'),
('G182', '60B-034.21', 'B01', 2, N'TRONG'), ('G183', '60B-034.21', 'B02', 2, N'TRONG'), ('G184', '60B-034.21', 'B03', 2, N'TRONG'), ('G185', '60B-034.21', 'B04', 2, N'TRONG'), ('G186', '60B-034.21', 'B05', 2, N'TRONG'), ('G187', '60B-034.21', 'B06', 2, N'TRONG'), ('G188', '60B-034.21', 'B07', 2, N'TRONG'), ('G189', '60B-034.21', 'B08', 2, N'TRONG'), ('G190', '60B-034.21', 'B09', 2, N'TRONG'), ('G191', '60B-034.21', 'B10', 2, N'TRONG'), ('G192', '60B-034.21', 'B11', 2, N'TRONG'), ('G193', '60B-034.21', 'B12', 2, N'TRONG'), ('G194', '60B-034.21', 'B13', 2, N'TRONG'), ('G195', '60B-034.21', 'B14', 2, N'TRONG'), ('G196', '60B-034.21', 'B15', 2, N'TRONG'), ('G197', '60B-034.21', 'B16', 2, N'TRONG'), ('G198', '60B-034.21', 'B17', 2, N'TRONG');

-- 7. Xe '51B-296.17' (Giường nằm VIP - 34 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G199', '51B-296.17', 'A01', 1, N'TRONG'), ('G200', '51B-296.17', 'A02', 1, N'TRONG'), ('G201', '51B-296.17', 'A03', 1, N'TRONG'), ('G202', '51B-296.17', 'A04', 1, N'TRONG'), ('G203', '51B-296.17', 'A05', 1, N'TRONG'), ('G204', '51B-296.17', 'A06', 1, N'TRONG'), ('G205', '51B-296.17', 'A07', 1, N'TRONG'), ('G206', '51B-296.17', 'A08', 1, N'TRONG'), ('G207', '51B-296.17', 'A09', 1, N'TRONG'), ('G208', '51B-296.17', 'A10', 1, N'TRONG'), ('G209', '51B-296.17', 'A11', 1, N'TRONG'), ('G210', '51B-296.17', 'A12', 1, N'TRONG'), ('G211', '51B-296.17', 'A13', 1, N'TRONG'), ('G212', '51B-296.17', 'A14', 1, N'TRONG'), ('G213', '51B-296.17', 'A15', 1, N'TRONG'), ('G214', '51B-296.17', 'A16', 1, N'TRONG'), ('G215', '51B-296.17', 'A17', 1, N'TRONG'),
('G216', '51B-296.17', 'B01', 2, N'TRONG'), ('G217', '51B-296.17', 'B02', 2, N'TRONG'), ('G218', '51B-296.17', 'B03', 2, N'TRONG'), ('G219', '51B-296.17', 'B04', 2, N'TRONG'), ('G220', '51B-296.17', 'B05', 2, N'TRONG'), ('G221', '51B-296.17', 'B06', 2, N'TRONG'), ('G222', '51B-296.17', 'B07', 2, N'TRONG'), ('G223', '51B-296.17', 'B08', 2, N'TRONG'), ('G224', '51B-296.17', 'B09', 2, N'TRONG'), ('G225', '51B-296.17', 'B10', 2, N'TRONG'), ('G226', '51B-296.17', 'B11', 2, N'TRONG'), ('G227', '51B-296.17', 'B12', 2, N'TRONG'), ('G228', '51B-296.17', 'B13', 2, N'TRONG'), ('G229', '51B-296.17', 'B14', 2, N'TRONG'), ('G230', '51B-296.17', 'B15', 2, N'TRONG'), ('G231', '51B-296.17', 'B16', 2, N'TRONG'), ('G232', '51B-296.17', 'B17', 2, N'TRONG');

-- 8. Xe 'XE-INT-01' (Nội thất Giường nằm đỏ - 20 Ghế mẫu - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G233', 'XE-INT-01', 'A01', 1, N'TRONG'), ('G234', 'XE-INT-01', 'A02', 1, N'TRONG'), ('G235', 'XE-INT-01', 'A03', 1, N'TRONG'), ('G236', 'XE-INT-01', 'A04', 1, N'TRONG'), ('G237', 'XE-INT-01', 'A05', 1, N'TRONG'), ('G238', 'XE-INT-01', 'A06', 1, N'TRONG'), ('G239', 'XE-INT-01', 'A07', 1, N'TRONG'), ('G240', 'XE-INT-01', 'A08', 1, N'TRONG'), ('G241', 'XE-INT-01', 'A09', 1, N'TRONG'), ('G242', 'XE-INT-01', 'A10', 1, N'TRONG'),
('G243', 'XE-INT-01', 'B01', 2, N'TRONG'), ('G244', 'XE-INT-01', 'B02', 2, N'TRONG'), ('G245', 'XE-INT-01', 'B03', 2, N'TRONG'), ('G246', 'XE-INT-01', 'B04', 2, N'TRONG'), ('G247', 'XE-INT-01', 'B05', 2, N'TRONG'), ('G248', 'XE-INT-01', 'B06', 2, N'TRONG'), ('G249', 'XE-INT-01', 'B07', 2, N'TRONG'), ('G250', 'XE-INT-01', 'B08', 2, N'TRONG'), ('G251', 'XE-INT-01', 'B09', 2, N'TRONG'), ('G252', 'XE-INT-01', 'B10', 2, N'TRONG');

-- 9. Xe 'XE-INT-02' (Nội thất Giường nằm cam - 20 Ghế mẫu - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G253', 'XE-INT-02', 'A01', 1, N'TRONG'), ('G254', 'XE-INT-02', 'A02', 1, N'TRONG'), ('G255', 'XE-INT-02', 'A03', 1, N'TRONG'), ('G256', 'XE-INT-02', 'A04', 1, N'TRONG'), ('G257', 'XE-INT-02', 'A05', 1, N'TRONG'), ('G258', 'XE-INT-02', 'A06', 1, N'TRONG'), ('G259', 'XE-INT-02', 'A07', 1, N'TRONG'), ('G260', 'XE-INT-02', 'A08', 1, N'TRONG'), ('G261', 'XE-INT-02', 'A09', 1, N'TRONG'), ('G262', 'XE-INT-02', 'A10', 1, N'TRONG'),
('G263', 'XE-INT-02', 'B01', 2, N'TRONG'), ('G264', 'XE-INT-02', 'B02', 2, N'TRONG'), ('G265', 'XE-INT-02', 'B03', 2, N'TRONG'), ('G266', 'XE-INT-02', 'B04', 2, N'TRONG'), ('G267', 'XE-INT-02', 'B05', 2, N'TRONG'), ('G268', 'XE-INT-02', 'B06', 2, N'TRONG'), ('G269', 'XE-INT-02', 'B07', 2, N'TRONG'), ('G270', 'XE-INT-02', 'B08', 2, N'TRONG'), ('G271', 'XE-INT-02', 'B09', 2, N'TRONG'), ('G272', 'XE-INT-02', 'B10', 2, N'TRONG');

-- 10. Xe 'XE-INT-03' (Cabin Cung điện Blue-LED - 22 Ghế - 2 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G273', 'XE-INT-03', 'P01', 1, N'TRONG'), ('G274', 'XE-INT-03', 'P02', 1, N'TRONG'), ('G275', 'XE-INT-03', 'P03', 1, N'TRONG'), ('G276', 'XE-INT-03', 'P04', 1, N'TRONG'), ('G277', 'XE-INT-03', 'P05', 1, N'TRONG'), ('G278', 'XE-INT-03', 'P06', 1, N'TRONG'), ('G279', 'XE-INT-03', 'P07', 1, N'TRONG'), ('G280', 'XE-INT-03', 'P08', 1, N'TRONG'), ('G281', 'XE-INT-03', 'P09', 1, N'TRONG'), ('G282', 'XE-INT-03', 'P10', 1, N'TRONG'), ('G283', 'XE-INT-03', 'P11', 1, N'TRONG'),
('G284', 'XE-INT-03', 'P12', 2, N'TRONG'), ('G285', 'XE-INT-03', 'P13', 2, N'TRONG'), ('G286', 'XE-INT-03', 'P14', 2, N'TRONG'), ('G287', 'XE-INT-03', 'P15', 2, N'TRONG'), ('G288', 'XE-INT-03', 'P16', 2, N'TRONG'), ('G289', 'XE-INT-03', 'P17', 2, N'TRONG'), ('G290', 'XE-INT-03', 'P18', 2, N'TRONG'), ('G291', 'XE-INT-03', 'P19', 2, N'TRONG'), ('G292', 'XE-INT-03', 'P20', 2, N'TRONG'), ('G293', 'XE-INT-03', 'P21', 2, N'TRONG'), ('G294', 'XE-INT-03', 'P22', 2, N'TRONG');

GO

-- 11. Xe '29B-555.99' (Limousine VIP - 9 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G295', '29B-555.99', '01', 1, N'TRONG'), ('G296', '29B-555.99', '02', 1, N'TRONG'), ('G297', '29B-555.99', '03', 1, N'TRONG'),
('G298', '29B-555.99', '04', 1, N'TRONG'), ('G299', '29B-555.99', '05', 1, N'TRONG'), ('G300', '29B-555.99', '06', 1, N'TRONG'),
('G301', '29B-555.99', '07', 1, N'TRONG'), ('G302', '29B-555.99', '08', 1, N'TRONG'), ('G303', '29B-555.99', '09', 1, N'TRONG');

-- 12. Xe '49F-005.75' (Limousine - 12 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G304', '49F-005.75', '01', 1, N'TRONG'), ('G305', '49F-005.75', '02', 1, N'TRONG'), ('G306', '49F-005.75', '03', 1, N'TRONG'),
('G307', '49F-005.75', '04', 1, N'TRONG'), ('G308', '49F-005.75', '05', 1, N'TRONG'), ('G309', '49F-005.75', '06', 1, N'TRONG'),
('G310', '49F-005.75', '07', 1, N'TRONG'), ('G311', '49F-005.75', '08', 1, N'TRONG'), ('G312', '49F-005.75', '09', 1, N'TRONG'),
('G313', '49F-005.75', '10', 1, N'TRONG'), ('G314', '49F-005.75', '11', 1, N'TRONG'), ('G315', '49F-005.75', '12', 1, N'TRONG');

-- 13. Xe '15B-999.01' (Dcar Limousine - 11 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G316', '15B-999.01', '01', 1, N'TRONG'), ('G317', '15B-999.01', '02', 1, N'TRONG'), ('G318', '15B-999.01', '03', 1, N'TRONG'),
('G319', '15B-999.01', '04', 1, N'TRONG'), ('G320', '15B-999.01', '05', 1, N'TRONG'), ('G321', '15B-999.01', '06', 1, N'TRONG'),
('G322', '15B-999.01', '07', 1, N'TRONG'), ('G323', '15B-999.01', '08', 1, N'TRONG'), ('G324', '15B-999.01', '09', 1, N'TRONG'),
('G325', '15B-999.01', '10', 1, N'TRONG'), ('G326', '15B-999.01', '11', 1, N'TRONG');

-- 14. Xe '47B-111.22' (Limousine Thương gia - 11 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G327', '47B-111.22', '01', 1, N'TRONG'), ('G328', '47B-111.22', '02', 1, N'TRONG'), ('G329', '47B-111.22', '03', 1, N'TRONG'),
('G330', '47B-111.22', '04', 1, N'TRONG'), ('G331', '47B-111.22', '05', 1, N'TRONG'), ('G332', '47B-111.22', '06', 1, N'TRONG'),
('G333', '47B-111.22', '07', 1, N'TRONG'), ('G334', '47B-111.22', '08', 1, N'TRONG'), ('G335', '47B-111.22', '09', 1, N'TRONG'),
('G336', '47B-111.22', '10', 1, N'TRONG'), ('G337', '47B-111.22', '11', 1, N'TRONG');

-- 15. Xe '30F-999.13' (Limousine Business - 7 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G338', '30F-999.13', '01', 1, N'TRONG'), ('G339', '30F-999.13', '02', 1, N'TRONG'), ('G340', '30F-999.13', '03', 1, N'TRONG'),
('G341', '30F-999.13', '04', 1, N'TRONG'), ('G342', '30F-999.13', '05', 1, N'TRONG'), ('G343', '30F-999.13', '06', 1, N'TRONG'),
('G344', '30F-999.13', '07', 1, N'TRONG');

-- 16. Xe '18B-007.89' (Limousine Purple - 9 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G345', '18B-007.89', '01', 1, N'TRONG'), ('G346', '18B-007.89', '02', 1, N'TRONG'), ('G347', '18B-007.89', '03', 1, N'TRONG'),
('G348', '18B-007.89', '04', 1, N'TRONG'), ('G349', '18B-007.89', '05', 1, N'TRONG'), ('G350', '18B-007.89', '06', 1, N'TRONG'),
('G351', '18B-007.89', '07', 1, N'TRONG'), ('G352', '18B-007.89', '08', 1, N'TRONG'), ('G353', '18B-007.89', '09', 1, N'TRONG');

-- 17. Xe '29B-888.18' (Limousine 19 chỗ - 19 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G354', '29B-888.18', '01', 1, N'TRONG'), ('G355', '29B-888.18', '02', 1, N'TRONG'), ('G356', '29B-888.18', '03', 1, N'TRONG'),
('G357', '29B-888.18', '04', 1, N'TRONG'), ('G358', '29B-888.18', '05', 1, N'TRONG'), ('G359', '29B-888.18', '06', 1, N'TRONG'),
('G360', '29B-888.18', '07', 1, N'TRONG'), ('G361', '29B-888.18', '08', 1, N'TRONG'), ('G362', '29B-888.18', '09', 1, N'TRONG'),
('G363', '29B-888.18', '10', 1, N'TRONG'), ('G364', '29B-888.18', '11', 1, N'TRONG'), ('G365', '29B-888.18', '12', 1, N'TRONG'),
('G366', '29B-888.18', '13', 1, N'TRONG'), ('G367', '29B-888.18', '14', 1, N'TRONG'), ('G368', '29B-888.18', '15', 1, N'TRONG'),
('G369', '29B-888.18', '16', 1, N'TRONG'), ('G370', '29B-888.18', '17', 1, N'TRONG'), ('G371', '29B-888.18', '18', 1, N'TRONG'), ('G372', '29B-888.18', '19', 1, N'TRONG');

-- 18. Xe '72B-004.56' (Ghế ngồi 45 chỗ Samco - 45 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G373', '72B-004.56', '01', 1, N'TRONG'), ('G374', '72B-004.56', '02', 1, N'TRONG'), ('G375', '72B-004.56', '03', 1, N'TRONG'), ('G376', '72B-004.56', '04', 1, N'TRONG'), ('G377', '72B-004.56', '05', 1, N'TRONG'),
('G378', '72B-004.56', '06', 1, N'TRONG'), ('G379', '72B-004.56', '07', 1, N'TRONG'), ('G380', '72B-004.56', '08', 1, N'TRONG'), ('G381', '72B-004.56', '09', 1, N'TRONG'), ('G382', '72B-004.56', '10', 1, N'TRONG'),
('G383', '72B-004.56', '11', 1, N'TRONG'), ('G384', '72B-004.56', '12', 1, N'TRONG'), ('G385', '72B-004.56', '13', 1, N'TRONG'), ('G386', '72B-004.56', '14', 1, N'TRONG'), ('G387', '72B-004.56', '15', 1, N'TRONG'),
('G388', '72B-004.56', '16', 1, N'TRONG'), ('G389', '72B-004.56', '17', 1, N'TRONG'), ('G390', '72B-004.56', '18', 1, N'TRONG'), ('G391', '72B-004.56', '19', 1, N'TRONG'), ('G392', '72B-004.56', '20', 1, N'TRONG'),
('G393', '72B-004.56', '21', 1, N'TRONG'), ('G394', '72B-004.56', '22', 1, N'TRONG'), ('G395', '72B-004.56', '23', 1, N'TRONG'), ('G396', '72B-004.56', '24', 1, N'TRONG'), ('G397', '72B-004.56', '25', 1, N'TRONG'),
('G398', '72B-004.56', '26', 1, N'TRONG'), ('G399', '72B-004.56', '27', 1, N'TRONG'), ('G400', '72B-004.56', '28', 1, N'TRONG'), ('G401', '72B-004.56', '29', 1, N'TRONG'), ('G402', '72B-004.56', '30', 1, N'TRONG'),
('G403', '72B-004.56', '31', 1, N'TRONG'), ('G404', '72B-004.56', '32', 1, N'TRONG'), ('G405', '72B-004.56', '33', 1, N'TRONG'), ('G406', '72B-004.56', '34', 1, N'TRONG'), ('G407', '72B-004.56', '35', 1, N'TRONG'),
('G408', '72B-004.56', '36', 1, N'TRONG'), ('G409', '72B-004.56', '37', 1, N'TRONG'), ('G410', '72B-004.56', '38', 1, N'TRONG'), ('G411', '72B-004.56', '39', 1, N'TRONG'), ('G412', '72B-004.56', '40', 1, N'TRONG'),
('G413', '72B-004.56', '41', 1, N'TRONG'), ('G414', '72B-004.56', '42', 1, N'TRONG'), ('G415', '72B-004.56', '43', 1, N'TRONG'), ('G416', '72B-004.56', '44', 1, N'TRONG'), ('G417', '72B-004.56', '45', 1, N'TRONG');

-- 19. Xe '50B-111.04' (Ghế ngồi 47 chỗ Daewoo - 47 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G418', '50B-111.04', '01', 1, N'TRONG'), ('G419', '50B-111.04', '02', 1, N'TRONG'), ('G420', '50B-111.04', '03', 1, N'TRONG'), ('G421', '50B-111.04', '04', 1, N'TRONG'), ('G422', '50B-111.04', '05', 1, N'TRONG'),
('G423', '50B-111.04', '06', 1, N'TRONG'), ('G424', '50B-111.04', '07', 1, N'TRONG'), ('G425', '50B-111.04', '08', 1, N'TRONG'), ('G426', '50B-111.04', '09', 1, N'TRONG'), ('G427', '50B-111.04', '10', 1, N'TRONG'),
('G428', '50B-111.04', '11', 1, N'TRONG'), ('G429', '50B-111.04', '12', 1, N'TRONG'), ('G430', '50B-111.04', '13', 1, N'TRONG'), ('G431', '50B-111.04', '14', 1, N'TRONG'), ('G432', '50B-111.04', '15', 1, N'TRONG'),
('G433', '50B-111.04', '16', 1, N'TRONG'), ('G434', '50B-111.04', '17', 1, N'TRONG'), ('G435', '50B-111.04', '18', 1, N'TRONG'), ('G436', '50B-111.04', '19', 1, N'TRONG'), ('G437', '50B-111.04', '20', 1, N'TRONG'),
('G438', '50B-111.04', '21', 1, N'TRONG'), ('G439', '50B-111.04', '22', 1, N'TRONG'), ('G440', '50B-111.04', '23', 1, N'TRONG'), ('G441', '50B-111.04', '24', 1, N'TRONG'), ('G442', '50B-111.04', '25', 1, N'TRONG'),
('G443', '50B-111.04', '26', 1, N'TRONG'), ('G444', '50B-111.04', '27', 1, N'TRONG'), ('G445', '50B-111.04', '28', 1, N'TRONG'), ('G446', '50B-111.04', '29', 1, N'TRONG'), ('G447', '50B-111.04', '30', 1, N'TRONG'),
('G448', '50B-111.04', '31', 1, N'TRONG'), ('G449', '50B-111.04', '32', 1, N'TRONG'), ('G450', '50B-111.04', '33', 1, N'TRONG'), ('G451', '50B-111.04', '34', 1, N'TRONG'), ('G452', '50B-111.04', '35', 1, N'TRONG'),
('G453', '50B-111.04', '36', 1, N'TRONG'), ('G454', '50B-111.04', '37', 1, N'TRONG'), ('G455', '50B-111.04', '38', 1, N'TRONG'), ('G456', '50B-111.04', '39', 1, N'TRONG'), ('G457', '50B-111.04', '40', 1, N'TRONG'),
('G458', '50B-111.04', '41', 1, N'TRONG'), ('G459', '50B-111.04', '42', 1, N'TRONG'), ('G460', '50B-111.04', '43', 1, N'TRONG'), ('G461', '50B-111.04', '44', 1, N'TRONG'), ('G462', '50B-111.04', '45', 1, N'TRONG'),
('G463', '50B-111.04', '46', 1, N'TRONG'), ('G464', '50B-111.04', '47', 1, N'TRONG');

-- 20. Xe '17B-002.11' (Ghế ngồi 29 chỗ Samco - 29 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G465', '17B-002.11', '01', 1, N'TRONG'), ('G466', '17B-002.11', '02', 1, N'TRONG'), ('G467', '17B-002.11', '03', 1, N'TRONG'), ('G468', '17B-002.11', '04', 1, N'TRONG'), ('G469', '17B-002.11', '05', 1, N'TRONG'),
('G470', '17B-002.11', '06', 1, N'TRONG'), ('G471', '17B-002.11', '07', 1, N'TRONG'), ('G472', '17B-002.11', '08', 1, N'TRONG'), ('G473', '17B-002.11', '09', 1, N'TRONG'), ('G474', '17B-002.11', '10', 1, N'TRONG'),
('G475', '17B-002.11', '11', 1, N'TRONG'), ('G476', '17B-002.11', '12', 1, N'TRONG'), ('G477', '17B-002.11', '13', 1, N'TRONG'), ('G478', '17B-002.11', '14', 1, N'TRONG'), ('G479', '17B-002.11', '15', 1, N'TRONG'),
('G480', '17B-002.11', '16', 1, N'TRONG'), ('G481', '17B-002.11', '17', 1, N'TRONG'), ('G482', '17B-002.11', '18', 1, N'TRONG'), ('G483', '17B-002.11', '19', 1, N'TRONG'), ('G484', '17B-002.11', '20', 1, N'TRONG'),
('G485', '17B-002.11', '21', 1, N'TRONG'), ('G486', '17B-002.11', '22', 1, N'TRONG'), ('G487', '17B-002.11', '23', 1, N'TRONG'), ('G488', '17B-002.11', '24', 1, N'TRONG'), ('G489', '17B-002.11', '25', 1, N'TRONG'),
('G490', '17B-002.11', '26', 1, N'TRONG'), ('G491', '17B-002.11', '27', 1, N'TRONG'), ('G492', '17B-002.11', '28', 1, N'TRONG'), ('G493', '17B-002.11', '29', 1, N'TRONG');

-- 21. Xe '43B-002.77' (Ghế ngồi 35 chỗ Thaco - 35 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G494', '43B-002.77', '01', 1, N'TRONG'), ('G495', '43B-002.77', '02', 1, N'TRONG'), ('G496', '43B-002.77', '03', 1, N'TRONG'), ('G497', '43B-002.77', '04', 1, N'TRONG'), ('G498', '43B-002.77', '05', 1, N'TRONG'),
('G499', '43B-002.77', '06', 1, N'TRONG'), ('G500', '43B-002.77', '07', 1, N'TRONG'), ('G501', '43B-002.77', '08', 1, N'TRONG'), ('G502', '43B-002.77', '09', 1, N'TRONG'), ('G503', '43B-002.77', '10', 1, N'TRONG'),
('G504', '43B-002.77', '11', 1, N'TRONG'), ('G505', '43B-002.77', '12', 1, N'TRONG'), ('G506', '43B-002.77', '13', 1, N'TRONG'), ('G507', '43B-002.77', '14', 1, N'TRONG'), ('G508', '43B-002.77', '15', 1, N'TRONG'),
('G509', '43B-002.77', '16', 1, N'TRONG'), ('G510', '43B-002.77', '17', 1, N'TRONG'), ('G511', '43B-002.77', '18', 1, N'TRONG'), ('G512', '43B-002.77', '19', 1, N'TRONG'), ('G513', '43B-002.77', '20', 1, N'TRONG'),
('G514', '43B-002.77', '21', 1, N'TRONG'), ('G515', '43B-002.77', '22', 1, N'TRONG'), ('G516', '43B-002.77', '23', 1, N'TRONG'), ('G517', '43B-002.77', '24', 1, N'TRONG'), ('G518', '43B-002.77', '25', 1, N'TRONG'),
('G519', '43B-002.77', '26', 1, N'TRONG'), ('G520', '43B-002.77', '27', 1, N'TRONG'), ('G521', '43B-002.77', '28', 1, N'TRONG'), ('G522', '43B-002.77', '29', 1, N'TRONG'), ('G523', '43B-002.77', '30', 1, N'TRONG'),
('G524', '43B-002.77', '31', 1, N'TRONG'), ('G525', '43B-002.77', '32', 1, N'TRONG'), ('G526', '43B-002.77', '33', 1, N'TRONG'), ('G527', '43B-002.77', '34', 1, N'TRONG'), ('G528', '43B-002.77', '35', 1, N'TRONG');

-- 22. Xe '65B-003.44' (Ghế ngồi 45 chỗ Hyundai - 45 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G529', '65B-003.44', '01', 1, N'TRONG'), ('G530', '65B-003.44', '02', 1, N'TRONG'), ('G531', '65B-003.44', '03', 1, N'TRONG'), ('G532', '65B-003.44', '04', 1, N'TRONG'), ('G533', '65B-003.44', '05', 1, N'TRONG'),
('G534', '65B-003.44', '06', 1, N'TRONG'), ('G535', '65B-003.44', '07', 1, N'TRONG'), ('G536', '65B-003.44', '08', 1, N'TRONG'), ('G537', '65B-003.44', '09', 1, N'TRONG'), ('G538', '65B-003.44', '10', 1, N'TRONG'),
('G539', '65B-003.44', '11', 1, N'TRONG'), ('G540', '65B-003.44', '12', 1, N'TRONG'), ('G541', '65B-003.44', '13', 1, N'TRONG'), ('G542', '65B-003.44', '14', 1, N'TRONG'), ('G543', '65B-003.44', '15', 1, N'TRONG'),
('G544', '65B-003.44', '16', 1, N'TRONG'), ('G545', '65B-003.44', '17', 1, N'TRONG'), ('G546', '65B-003.44', '18', 1, N'TRONG'), ('G547', '65B-003.44', '19', 1, N'TRONG'), ('G548', '65B-003.44', '20', 1, N'TRONG'),
('G549', '65B-003.44', '21', 1, N'TRONG'), ('G550', '65B-003.44', '22', 1, N'TRONG'), ('G551', '65B-003.44', '23', 1, N'TRONG'), ('G552', '65B-003.44', '24', 1, N'TRONG'), ('G553', '65B-003.44', '25', 1, N'TRONG'),
('G554', '65B-003.44', '26', 1, N'TRONG'), ('G555', '65B-003.44', '27', 1, N'TRONG'), ('G556', '65B-003.44', '28', 1, N'TRONG'), ('G557', '65B-003.44', '29', 1, N'TRONG'), ('G558', '65B-003.44', '30', 1, N'TRONG'),
('G559', '65B-003.44', '31', 1, N'TRONG'), ('G560', '65B-003.44', '32', 1, N'TRONG'), ('G561', '65B-003.44', '33', 1, N'TRONG'), ('G562', '65B-003.44', '34', 1, N'TRONG'), ('G563', '65B-003.44', '35', 1, N'TRONG'),
('G564', '65B-003.44', '36', 1, N'TRONG'), ('G565', '65B-003.44', '37', 1, N'TRONG'), ('G566', '65B-003.44', '38', 1, N'TRONG'), ('G567', '65B-003.44', '39', 1, N'TRONG'), ('G568', '65B-003.44', '40', 1, N'TRONG'),
('G569', '65B-003.44', '41', 1, N'TRONG'), ('G570', '65B-003.44', '42', 1, N'TRONG'), ('G571', '65B-003.44', '43', 1, N'TRONG'), ('G572', '65B-003.44', '44', 1, N'TRONG'), ('G573', '65B-003.44', '45', 1, N'TRONG');

-- 23. Xe '16B-637.74' (Hyundai Global - 29 Ghế - 1 Tầng)
INSERT INTO GHE (maGhe, bienSo, soGhe, tang, trangThai) VALUES 
('G574', '16B-637.74', '01', 1, N'TRONG'), ('G575', '16B-637.74', '02', 1, N'TRONG'), ('G576', '16B-637.74', '03', 1, N'TRONG'), ('G577', '16B-637.74', '04', 1, N'TRONG'), ('G578', '16B-637.74', '05', 1, N'TRONG'),
('G579', '16B-637.74', '06', 1, N'TRONG'), ('G580', '16B-637.74', '07', 1, N'TRONG'), ('G581', '16B-637.74', '08', 1, N'TRONG'), ('G582', '16B-637.74', '09', 1, N'TRONG'), ('G583', '16B-637.74', '10', 1, N'TRONG'),
('G584', '16B-637.74', '11', 1, N'TRONG'), ('G585', '16B-637.74', '12', 1, N'TRONG'), ('G586', '16B-637.74', '13', 1, N'TRONG'), ('G587', '16B-637.74', '14', 1, N'TRONG'), ('G588', '16B-637.74', '15', 1, N'TRONG'),
('G589', '16B-637.74', '16', 1, N'TRONG'), ('G590', '16B-637.74', '17', 1, N'TRONG'), ('G591', '16B-637.74', '18', 1, N'TRONG'), ('G592', '16B-637.74', '19', 1, N'TRONG'), ('G593', '16B-637.74', '20', 1, N'TRONG'),
('G594', '16B-637.74', '21', 1, N'TRONG'), ('G595', '16B-637.74', '22', 1, N'TRONG'), ('G596', '16B-637.74', '23', 1, N'TRONG'), ('G597', '16B-637.74', '24', 1, N'TRONG'), ('G598', '16B-637.74', '25', 1, N'TRONG'),
('G599', '16B-637.74', '26', 1, N'TRONG'), ('G600', '16B-637.74', '27', 1, N'TRONG'), ('G601', '16B-637.74', '28', 1, N'TRONG'), ('G602', '16B-637.74', '29', 1, N'TRONG');

GO
INSERT INTO BENXE (maBenXe, maPhuongNo, tenBenXe, diaChi, sdt) VALUES 
-- Miền Bắc
('BX01', 'PX01', N'Bến xe Cao Bằng', N'Số 01, P. Sông Bằng, Cao Bằng', '02063852432'),
('BX02', 'PX03', N'Bến xe Lào Cai', N'Phường Lào Cai, TP. Lào Cai', '02143835222'),
('BX03', 'PX05', N'Bến xe Yên Bái', N'P. Đồng Tâm, TP. Yên Bái', '02163851231'),
('BX04', 'PX07', N'Bến xe Sơn La', N'P. Chiềng Lề, TP. Sơn La', '02123852321'),
('BX05', 'PX09', N'Bến xe Hà Giang', N'P. Minh Khai, TP. Hà Giang', '02193863256'),
('BX06', 'PX11', N'Bến xe Lai Châu', N'P. Quyết Tiến, TP. Lai Châu', '02133875234'),
('BX07', 'PX14', N'Bến xe Mỹ Đình', N'20 Phạm Hùng, Mỹ Đình 1, Nam Từ Liêm, Hà Nội', '02437685549'),
('BX08', 'PX15', N'Bến xe Vinh', N'77 Lê Lợi, TP. Vinh, Nghệ An', '02383835182'),
('BX09', 'PX17', N'Bến xe Nam Định', N'Giải Phóng, P. Vị Xuyên, Nam Định', '02283849233'),
('BX10', 'PX19', N'Bến xe Ninh Bình', N'P. Vân Giang, TP. Ninh Bình', '02293871321'),
('BX11', 'PX21', N'Bến xe Việt Trì', N'P. Gia Cẩm, Việt Trì, Phú Thọ', '02103846321'),
('BX12', 'PX23', N'Bến xe Đồng Hới', N'Trần Hưng Đạo, P. Đồng Hải, Quảng Bình', '02323822164'),

-- Miền Trung
('BX13', 'PX25', N'Bến xe Phía Nam Huế', N'97 An Dương Vương, P. Phú Hội, Huế', '02343823814'),
('BX14', 'PX27', N'Bến xe Đông Hà', N'P. 1, Đông Hà, Quảng Trị', '02333851221'),
('BX15', 'PX29', N'Bến xe Tam Kỳ', N'P. Tân Thạnh, Tam Kỳ, Quảng Nam', '02353851413'),
('BX16', 'PX31', N'Bến xe Quảng Ngãi', N'02 Trần Khánh Dư, P. Quảng Phú', '02553822895'),
('BX17', 'PX33', N'Bến xe Quy Nhơn', N'71 Tây Sơn, Quy Nhơn, Bình Định', '02563842273'),
('BX18', 'PX35', N'Bến xe Trung tâm Đà Nẵng', N'201 Tôn Đức Thắng, Hòa Minh, Đà Nẵng', '02363767447'),
('BX19', 'PX37', N'Bến xe Kon Tum', N'281 Phan Đình Phùng, TP. Kon Tum', '02603862308'),
('BX20', 'PX39', N'Bến xe Bãi Cháy', N'P. Bạch Đằng, Hạ Long, Quảng Ninh', '02033844411'),
('BX21', 'PX41', N'Bến xe Tuy Hòa', N'P. 1, Tuy Hòa, Phú Yên', '02573823238'),
('BX22', 'PX43', N'Bến xe Phía Nam Nha Trang', N'P. Lộc Thọ, Nha Trang, Khánh Hòa', '02583812812'),
('BX23', 'PX45', N'Bến xe Phan Rang', N'P. Mỹ Hải, Phan Rang, Ninh Thuận', '02593822146'),

-- Tây Nguyên & Miền Nam
('BX24', 'PX47', N'Bến xe Liên tỉnh Đà Lạt', N'01 Tô Hiến Thành, P.1, Đà Lạt', '02633822121'),
('BX25', 'PX49', N'Bến xe Phía Nam Buôn Ma Thuột', N'Võ Văn Kiệt, P. Tân Lập, Đắk Lắk', '02623811638'),
('BX26', 'PX51', N'Bến xe Miền Đông', N'292 Đinh Bộ Lĩnh, P. Bến Nghé, Quận 1, TP.HCM', '02838994056'),
('BX27', 'PX53', N'Bến xe Vũng Tàu', N'192 Nam Kỳ Khởi Nghĩa, Vũng Tàu', '02543859727'),
('BX28', 'PX55', N'Bến xe Bình Dương', N'P. Phú Cường, Thủ Dầu Một', '02743822280'),
('BX29', 'PX57', N'Bến xe Phan Thiết', N'P. Mũi Né, Phan Thiết, Bình Thuận', '02523821212'),
('BX30', 'PX59', N'Bến xe Đức Long Gia Lai', N'43 Lý Nam Đế, P. Tây Sơn, Pleiku', '02693824415'),
('BX31', 'PX61', N'Bến xe Rạch Giá', N'P. Vĩnh Thanh Vân, Kiên Giang', '02973863299'),
('BX32', 'PX63', N'Bến xe Trung tâm Cần Thơ', N'P. Tân An, Q. Ninh Kiều, Cần Thơ', '02923769769'),
('BX33', 'PX65', N'Bến xe Biên Hòa', N'P. Thanh Bình, Biên Hòa, Đồng Nai', '02513822340'),
('BX34', 'PX67', N'Bến xe Long Xuyên', N'P. Mỹ Long, Long Xuyên, An Giang', '02963852124');

GO

/* =============================================================
   4. TUYENDUONG - Kết nối các bến xe trọng điểm
   Quy tắc mã: TD + [Số thứ tự]
============================================================= */
INSERT INTO TUYENDUONG (maTuyen, maBenDi, maBenDen, khoangCach, thoiGianDuKien) VALUES 
('TD01', 'BX07', 'BX26', 1720.00, 1920), -- Hà Nội -> TP.HCM
('TD02', 'BX26', 'BX07', 1720.00, 1920), -- TP.HCM -> Hà Nội
('TD03', 'BX26', 'BX24', 305.00, 420),   -- TP.HCM -> Đà Lạt
('TD04', 'BX24', 'BX26', 305.00, 420),   -- Đà Lạt -> TP.HCM
('TD05', 'BX26', 'BX18', 960.00, 1020),  -- TP.HCM -> Đà Nẵng
('TD06', 'BX18', 'BX26', 960.00, 1020),  -- Đà Nẵng -> TP.HCM
('TD07', 'BX26', 'BX25', 350.00, 480),   -- TP.HCM -> Buôn Ma Thuột
('TD08', 'BX26', 'BX32', 170.00, 210),   -- TP.HCM -> Cần Thơ
('TD09', 'BX07', 'BX02', 320.00, 360),   -- Hà Nội -> Lào Cai
('TD10', 'BX08', 'BX07', 300.00, 360),   -- Nghệ An -> Hà Nội
('TD11', 'BX26', 'BX22', 440.00, 540),   -- TP.HCM -> Nha Trang
('TD12', 'BX22', 'BX24', 140.00, 240),   -- Nha Trang -> Đà Lạt
-- Tuyến Miền Bắc & Đông Bắc
('TD13', 'BX07', 'BX01', 280.00, 360),   -- Hà Nội -> Cao Bằng
('TD14', 'BX07', 'BX05', 300.00, 420),   -- Hà Nội -> Hà Giang
('TD15', 'BX02', 'BX07', 320.00, 360),   -- Lào Cai -> Hà Nội

-- Tuyến Miền Trung & Tây Nguyên
('TD16', 'BX18', 'BX13', 100.00, 120),   -- Đà Nẵng -> Huế
('TD17', 'BX18', 'BX30', 330.00, 450),   -- Đà Nẵng -> Gia Lai (Pleiku)
('TD18', 'BX30', 'BX25', 180.00, 240),   -- Gia Lai -> Buôn Ma Thuột
('TD19', 'BX25', 'BX24', 210.00, 300),   -- Buôn Ma Thuột -> Đà Lạt

-- Tuyến Miền Nam & Miền Tây
('TD20', 'BX26', 'BX27', 100.00, 120),   -- TP.HCM -> Vũng Tàu
('TD21', 'BX26', 'BX34', 190.00, 240),   -- TP.HCM -> An Giang
('TD22', 'BX32', 'BX31', 115.00, 150),   -- Cần Thơ -> Rạch Giá
('TD23', 'BX26', 'BX29', 200.00, 270),   -- TP.HCM -> Phan Thiết
('TD24', 'BX22', 'BX17', 220.00, 300);   -- Nha Trang -> Quy Nhơn
go
/* =============================================================
   11. CHUYENXE - Đảm bảo 23 xe từ 23 ảnh đều tham gia chạy
   Quy tắc mã: CX + [Số thứ tự]
============================================================= */
INSERT INTO CHUYENXE (maChuyen, maTuyen, bienSo, ngayDi, gioDi, giaVe, trangThai) VALUES 
-- Tuyến Bắc - Nam (TD01, TD02)
('CX01', 'TD01', '50E-802.84', '2026-06-15', '18:00', 850000, N'SAPDI'),
('CX02', 'TD02', '51B-115.00', '2026-06-15', '19:00', 900000, N'SAPDI'), -- [FIX] Sửa 115.01 thành 115.00
('CX03', 'TD01', '38B-012.42', '2026-06-16', '07:00', 750000, N'SAPDI'),

-- Tuyến TP.HCM - Đà Lạt (TD03, TD04)
('CX04', 'TD03', '49F-005.76', '2026-06-15', '08:00', 300000, N'SAPDI'),
('CX05', 'TD04', '60B-034.21', '2026-06-15', '22:00', 350000, N'SAPDI'),
('CX06', 'TD03', '51B-296.17', '2026-06-16', '12:00', 320000, N'SAPDI'),

-- Tuyến TP.HCM - Đà Nẵng (TD05, TD06)
('CX07', 'TD05', '72B-004.56', '2026-06-15', '09:00', 450000, N'SAPDI'),
('CX08', 'TD06', '65B-003.44', '2026-06-15', '15:30', 450000, N'SAPDI'),

-- Tuyến Limousine VIP / Tuyến ngắn (TD07, TD08, TD09, TD11)
('CX09', 'TD08', '29B-555.99', '2026-06-15', '05:00', 250000, N'SAPDI'),
('CX10', 'TD07', '49F-005.75', '2026-06-15', '13:00', 380000, N'SAPDI'),
('CX11', 'TD09', '15B-999.01', '2026-06-16', '06:30', 400000, N'SAPDI'), -- [FIX] Sửa 15B-009.01 thành 15B-999.01
('CX12', 'TD11', '47B-111.22', '2026-06-15', '21:00', 420000, N'SAPDI'),
('CX13', 'TD08', '30F-999.13', '2026-06-16', '14:00', 280000, N'SAPDI'),
('CX14', 'TD09', '16B-637.74', '2026-06-15', '08:00', 350000, N'SAPDI'),
('CX15', 'TD11', '29B-888.18', '2026-06-16', '23:00', 450000, N'SAPDI'),

-- Các xe còn lại chạy các tuyến bổ sung
('CX16', 'TD10', '53S-8471',   '2026-06-15', '20:00', 220000, N'SAPDI'),
('CX17', 'TD12', '60B-034.21', '2026-06-15', '10:00', 500000, N'SAPDI'), -- [FIX] Sửa biển 222.33 ảo thành 60B-034.21
('CX18', 'TD01', '50B-111.04', '2026-06-17', '18:00', 700000, N'SAPDI'),
('CX19', 'TD02', '17B-002.11', '2026-06-17', '19:00', 700000, N'SAPDI'),
('CX20', 'TD03', '43B-002.77', '2026-06-17', '08:00', 250000, N'SAPDI'),
('CX21', 'TD05', '18B-007.89', '2026-06-17', '14:00', 480000, N'SAPDI'),
('CX22', 'TD09', 'XE-INT-03',  '2026-06-17', '21:00', 600000, N'SAPDI'), -- [FIX] Sửa biển 24B ảo thành XE-INT-03
('CX23', 'TD04', 'XE-INT-01',  '2026-06-16', '22:00', 300000, N'SAPDI'),
('CX24', 'TD13', '15B-999.01', '2026-06-18', '07:00', 300000, N'SAPDI'), -- [FIX] Sửa 009.01 thành 999.01
('CX25', 'TD14', '16B-637.74', '2026-06-18', '20:00', 350000, N'SAPDI'),
('CX26', 'TD15', 'XE-INT-03',  '2026-06-18', '22:00', 450000, N'SAPDI'), -- [FIX] Sửa biển 24B ảo thành XE-INT-03

-- Các tuyến Tây Nguyên
('CX27', 'TD17', '60B-034.21', '2026-06-18', '08:30', 400000, N'SAPDI'), -- [FIX] Sửa biển 222.33 ảo thành 60B-034.21
('CX28', 'TD18', '18B-007.89', '2026-06-19', '09:00', 200000, N'SAPDI'),
('CX29', 'TD19', '51B-296.17', '2026-06-19', '14:00', 250000, N'SAPDI'),

-- Các tuyến Miền Nam & Miền Tây
('CX30', 'TD20', '29B-888.18', '2026-06-18', '15:00', 180000, N'SAPDI'),
('CX31', 'TD21', '43B-002.77', '2026-06-18', '05:00', 220000, N'SAPDI'),
('CX32', 'TD22', '17B-002.11', '2026-06-18', '06:00', 150000, N'SAPDI'),

-- Các tuyến bổ sung cho xe nội thất đặc biệt
('CX33', 'TD23', 'XE-INT-02', '2026-06-19', '18:00', 300000, N'SAPDI'),
('CX34', 'TD24', 'XE-INT-01', '2026-06-19', '13:00', 280000, N'SAPDI');
GO
INSERT INTO KHUYENMAI (maKM, tenKhuyenMai, loaiKM, giaTriGiam, ngayBatDau, ngayKetThuc) VALUES 
-- Giảm theo phần trăm (Tối đa 100%)
('KM01', N'Chào hè rực rỡ 2026', N'PHANTRAM', 10.00, '2026-05-01 00:00:00', '2026-08-31 23:59:59'),
('KM02', N'Ưu đãi Tết Nguyên Đán', N'PHANTRAM', 5.00, '2026-01-15 00:00:00', '2026-02-15 23:59:59'),
('KM03', N'Tri ân khách hàng thân thiết', N'PHANTRAM', 15.00, '2026-01-01 00:00:00', '2026-12-31 23:59:59'),
('KM04', N'Ngày hội học sinh sinh viên', N'PHANTRAM', 20.00, '2026-08-15 00:00:00', '2026-09-30 23:59:59'),

-- Giảm theo số tiền cố định (VNĐ)
('KM05', N'Khai trương tuyến Hà Nội - Cao Bằng', N'CODINH', 50000.00, '2026-06-01 00:00:00', '2026-06-30 23:59:59'),
('KM06', N'Giảm giá đặt vé qua App lần đầu', N'CODINH', 30000.00, '2026-01-01 00:00:00', '2026-12-31 23:59:59'),
('KM07', N'Ưu đãi lễ Quốc khánh 2/9', N'CODINH', 100000.00, '2026-08-25 00:00:00', '2026-09-05 23:59:59'),
('KM08', N'Khuyến mãi chặng TP.HCM - Vũng Tàu', N'CODINH', 20000.00, '2026-06-15 00:00:00', '2026-07-15 23:59:59');

GO

/* =============================================================
   12. DONDATVE - 7 Đơn hàng mẫu
============================================================= */
INSERT INTO DONDATVE (maDon, maKH, maNV, ngayDat, tongTien, tienCoc, trangThai, ghiChu) VALUES 
('DON01', 'KH01', 'NV01', '2026-05-10 08:00', 850000, 850000, N'DAXACNHAN', N'Khách đi 1 người - Limousine VIP'),
('DON02', 'KH02', 'NV01', '2026-05-10 09:15', 600000, 300000, N'CHOXULY',   N'Khách đi 2 người - Giường nằm - Cọc 50%'),
('DON03', 'KH03', 'NV03', '2026-05-10 10:30', 1350000, 1350000, N'HOANTHANH', N'Khách đoàn 3 người - Ghế ngồi Samco'),
('DON04', 'KH04', NULL,   '2026-05-10 14:00', 250000, 0,      N'CHOXULY',   N'Đặt 1 ghế qua App'),
('DON05', 'KH05', 'NV02', '2026-05-10 15:45', 1700000, 1700000, N'DAXACNHAN', N'Cặp đôi đặt 2 phòng VIP'),
('DON06', 'KH06', 'NV03', '2026-05-10 16:20', 380000, 380000, N'DAXACNHAN', N'Khách đi 1 người - Solati'),
('DON07', 'KH07', NULL,   '2026-05-10 20:00', 900000, 450000, N'CHOXULY',   N'Đặt 3 ghế - Giường nằm');
go

/* =============================================================
   13. CHITIETDATVE - Chi tiết ghế
============================================================= */
-- Đơn 01: 1 ghế VIP (Xe 50E-802.84)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) 
VALUES ('CT01', 'DON01', 'CX01', 'G081', 850000, N'DADAT');

-- Đơn 02: 2 ghế giường nằm (Xe 49F-005.76, mã ghế từ G001-G040)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) VALUES 
('CT02', 'DON02', 'CX04', 'G001', 300000, N'DADAT'),
('CT03', 'DON02', 'CX04', 'G002', 300000, N'DADAT');

-- Đơn 03: 3 ghế ngồi Samco (Xe 72B-004.56, mã ghế từ G373-G417)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) VALUES 
('CT04', 'DON03', 'CX07', 'G373', 450000, N'DASUDUNG'),
('CT05', 'DON03', 'CX07', 'G374', 450000, N'DASUDUNG'),
('CT06', 'DON03', 'CX07', 'G375', 450000, N'DASUDUNG');

-- Đơn 04: 1 ghế Limousine S1 (Xe 29B-555.99, mã ghế từ G295-G303)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) 
VALUES ('CT07', 'DON04', 'CX09', 'G295', 250000, N'DADAT');

-- Đơn 05: 2 phòng VIP (Xe 51B-115.00, mã ghế từ G103-G124)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) VALUES 
('CT08', 'DON05', 'CX02', 'G103', 850000, N'DADAT'),
('CT09', 'DON05', 'CX02', 'G104', 850000, N'DADAT');

-- Đơn 06: 1 ghế Limousine Solati (Xe 49F-005.75, mã ghế từ G304-G315)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) 
VALUES ('CT10', 'DON06', 'CX10', 'G304', 380000, N'DADAT');

-- Đơn 07: 3 ghế giường nằm (Xe XE-INT-01, mã ghế từ G233-G252)
INSERT INTO CHITIETDATVE (maCTDat, maDon, maChuyen, maGhe, giaVeLucDat, trangThaiVe) VALUES 
('CT11', 'DON07', 'CX23', 'G233', 300000, N'DADAT'),
('CT12', 'DON07', 'CX23', 'G234', 300000, N'DADAT'),
('CT13', 'DON07', 'CX23', 'G235', 300000, N'DADAT');
GO

/* =============================================================
   14. THANHTOAN - Ghi nhận dòng tiền
============================================================= */
INSERT INTO THANHTOAN (maTT, maDon, soTien, phuongThuc, thoiGianTT, trangThai) VALUES 
('TT01', 'DON01', 850000,  N'CHUYENKHOAN', '2026-05-10 08:05', N'THANHCONG'),
('TT02', 'DON02', 300000,  N'TIENMAT',      '2026-05-10 09:20', N'THANHCONG'),
('TT03', 'DON03', 1350000, N'CHUYENKHOAN', '2026-05-10 10:35', N'THANHCONG'),
('TT04', 'DON05', 1700000, N'CHUYENKHOAN', '2026-05-10 15:50', N'THANHCONG'),
('TT05', 'DON06', 380000,  N'TIENMAT',      '2026-05-10 16:25', N'THANHCONG'),
('TT06', 'DON07', 450000,  N'CHUYENKHOAN', '2026-05-10 20:05', N'THANHCONG');
GO
