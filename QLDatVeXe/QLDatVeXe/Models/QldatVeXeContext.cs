using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLDatVeXe.Models;

public partial class QldatVeXeContext : DbContext
{
    public QldatVeXeContext()
    {
    }

    public QldatVeXeContext(DbContextOptions<QldatVeXeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Benxe> Benxe { get; set; }

    public virtual DbSet<Chitietdatve> Chitietdatve { get; set; }

    public virtual DbSet<Chuyenxe> Chuyenxe { get; set; }

    public virtual DbSet<Danhgia> Danhgia { get; set; }

    public virtual DbSet<Dondatve> Dondatve { get; set; }

    public virtual DbSet<DondatveKhuyenmai> DondatveKhuyenmai { get; set; }

    public virtual DbSet<Ghe> Ghe { get; set; }

    public virtual DbSet<Khachhang> Khachhang { get; set; }

    public virtual DbSet<Khuyenmai> Khuyenmai { get; set; }

    public virtual DbSet<Nhanvien> Nhanvien { get; set; }

    public virtual DbSet<Nhaxe> Nhaxe { get; set; }

    public virtual DbSet<Phuongxa> Phuongxa { get; set; }

    public virtual DbSet<Thanhtoan> Thanhtoan { get; set; }

    public virtual DbSet<Tinhthanh> Tinhthanh { get; set; }

    public virtual DbSet<Tuyenduong> Tuyenduong { get; set; }

    public virtual DbSet<VChuyenxe> VChuyenxe { get; set; }

    public virtual DbSet<VDoanhthu> VDoanhthu { get; set; }

    public virtual DbSet<Xe> Xe { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Benxe>(entity =>
        {
            entity.HasKey(e => e.MaBenXe).HasName("PK__BENXE__5B948C836E47750A");

            entity.ToTable("BENXE");

            entity.Property(e => e.MaBenXe)
                .HasMaxLength(50)
                .HasColumnName("maBenXe");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.MaPhuongNo)
                .HasMaxLength(50)
                .HasColumnName("maPhuongNo");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
            entity.Property(e => e.TenBenXe)
                .HasMaxLength(150)
                .HasColumnName("tenBenXe");

            entity.HasOne(d => d.MaPhuongNoNavigation).WithMany(p => p.Benxe)
                .HasForeignKey(d => d.MaPhuongNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BENXE_PHUONG");
        });

        modelBuilder.Entity<Chitietdatve>(entity =>
        {
            entity.HasKey(e => e.MaCtdat).HasName("PK__CHITIETD__EBDBB76E79DF7FE3");

            entity.ToTable("CHITIETDATVE", tb => tb.HasTrigger("TRG_CTDV_KIEMTRA_GHE_XE"));

            entity.HasIndex(e => e.MaChuyen, "IX_CTDV_CHUYEN");

            entity.HasIndex(e => e.TrangThaiVe, "IX_CTDV_TRANGTHAIDE");

            entity.Property(e => e.MaCtdat)
                .HasMaxLength(50)
                .HasColumnName("maCTDat");
            entity.Property(e => e.GiaVeLucDat)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("giaVeLucDat");
            entity.Property(e => e.MaChuyen)
                .HasMaxLength(50)
                .HasColumnName("maChuyen");
            entity.Property(e => e.MaDon)
                .HasMaxLength(50)
                .HasColumnName("maDon");
            entity.Property(e => e.MaGhe)
                .HasMaxLength(50)
                .HasColumnName("maGhe");
            entity.Property(e => e.TrangThaiVe)
                .HasMaxLength(20)
                .HasDefaultValue("DADAT")
                .HasColumnName("trangThaiVe");

            entity.HasOne(d => d.MaChuyenNavigation).WithMany(p => p.Chitietdatve)
                .HasForeignKey(d => d.MaChuyen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDV_CHUYEN");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.Chitietdatve)
                .HasForeignKey(d => d.MaDon)
                .HasConstraintName("FK_CTDV_DON");

            entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.Chitietdatve)
                .HasForeignKey(d => d.MaGhe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDV_GHE");
        });

        modelBuilder.Entity<Chuyenxe>(entity =>
        {
            entity.HasKey(e => e.MaChuyen).HasName("PK__CHUYENXE__743488A7EC4AFFBC");

            entity.ToTable("CHUYENXE", tb => tb.HasTrigger("TRG_CHUYENXE_AFTER_INSERT_UPDATE"));

            entity.HasIndex(e => e.NgayDi, "IX_CHUYENXE_NGAY");

            entity.HasIndex(e => e.TrangThai, "IX_CHUYENXE_TRANG");

            entity.Property(e => e.MaChuyen)
                .HasMaxLength(50)
                .HasColumnName("maChuyen");
            entity.Property(e => e.BienSo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("bienSo");
            entity.Property(e => e.GiaVe)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("giaVe");
            entity.Property(e => e.GioDi).HasColumnName("gioDi");
            entity.Property(e => e.MaTuyen)
                .HasMaxLength(50)
                .HasColumnName("maTuyen");
            entity.Property(e => e.NgayDi).HasColumnName("ngayDi");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("SAPDI")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.BienSoNavigation).WithMany(p => p.Chuyenxe)
                .HasForeignKey(d => d.BienSo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHUYENXE_XE");

            entity.HasOne(d => d.MaTuyenNavigation).WithMany(p => p.Chuyenxe)
                .HasForeignKey(d => d.MaTuyen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHUYENXE_TUYEN");
        });

        modelBuilder.Entity<Danhgia>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DANHGIA__6B15DD9A0C48C07A");

            entity.ToTable("DANHGIA");

            entity.HasIndex(e => e.MaChuyen, "IX_DANHGIA_CHUYEN");

            entity.HasIndex(e => new { e.MaKh, e.MaChuyen }, "UQ_DANHGIA_KH_CHUYEN").IsUnique();

            entity.Property(e => e.MaDanhGia)
                .HasMaxLength(50)
                .HasColumnName("maDanhGia");
            entity.Property(e => e.BinhLuan)
                .HasMaxLength(500)
                .HasColumnName("binhLuan");
            entity.Property(e => e.DiemDanhGia).HasColumnName("diemDanhGia");
            entity.Property(e => e.MaChuyen)
                .HasMaxLength(50)
                .HasColumnName("maChuyen");
            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .HasColumnName("maKH");
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayDanhGia");

            entity.HasOne(d => d.MaChuyenNavigation).WithMany(p => p.Danhgia)
                .HasForeignKey(d => d.MaChuyen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DANHGIA_CHUYEN");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Danhgia)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DANHGIA_KH");
        });

        modelBuilder.Entity<Dondatve>(entity =>
        {
            entity.HasKey(e => e.MaDon).HasName("PK__DONDATVE__2431086D740ADAC0");

            entity.ToTable("DONDATVE");

            entity.HasIndex(e => e.MaKh, "IX_DONDATVE_KH");

            entity.Property(e => e.MaDon)
                .HasMaxLength(50)
                .HasColumnName("maDon");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(255)
                .HasColumnName("ghiChu");
            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .HasColumnName("maKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(50)
                .HasColumnName("maNV");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayDat");
            entity.Property(e => e.Sdtnguoidi)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("sdtnguoidi");
            entity.Property(e => e.Tennguoidi)
                .HasMaxLength(100)
                .HasColumnName("tennguoidi");
            entity.Property(e => e.TienCoc)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("tienCoc");
            entity.Property(e => e.TongTien)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("tongTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("CHOXULY")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Dondatve)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DON_KH");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Dondatve)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DON_NV");
        });

        modelBuilder.Entity<DondatveKhuyenmai>(entity =>
        {
            entity.HasKey(e => new { e.MaDon, e.MaKm }).HasName("PK__DONDATVE__C392E492A035C2D1");

            entity.ToTable("DONDATVE_KHUYENMAI");

            entity.Property(e => e.MaDon)
                .HasMaxLength(50)
                .HasColumnName("maDon");
            entity.Property(e => e.MaKm)
                .HasMaxLength(50)
                .HasColumnName("maKM");
            entity.Property(e => e.SoTienGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("soTienGiam");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.DondatveKhuyenmai)
                .HasForeignKey(d => d.MaDon)
                .HasConstraintName("FK_DDVKM_DON");

            entity.HasOne(d => d.MaKmNavigation).WithMany(p => p.DondatveKhuyenmai)
                .HasForeignKey(d => d.MaKm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DDVKM_KM");
        });

        modelBuilder.Entity<Ghe>(entity =>
        {
            entity.HasKey(e => e.MaGhe).HasName("PK__GHE__2D87404C9A377519");

            entity.ToTable("GHE");

            entity.HasIndex(e => new { e.BienSo, e.SoGhe }, "UQ_GHE").IsUnique();

            entity.Property(e => e.MaGhe)
                .HasMaxLength(50)
                .HasColumnName("maGhe");
            entity.Property(e => e.BienSo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("bienSo");
            entity.Property(e => e.SoGhe)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("soGhe");
            entity.Property(e => e.Tang)
                .HasDefaultValue(1)
                .HasColumnName("tang");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("TRONG")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.BienSoNavigation).WithMany(p => p.Ghe)
                .HasForeignKey(d => d.BienSo)
                .HasConstraintName("FK_GHE_XE");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACHHAN__7A3ECFE496EA1588");

            entity.ToTable("KHACHHANG");

            entity.HasIndex(e => e.TenDangNhap, "UQ__KHACHHAN__59267D4A73A764FF").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__KHACHHAN__DDDFB483DBDAF89C").IsUnique();

            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .HasColumnName("maKH");
            entity.Property(e => e.GioiTinh).HasColumnName("gioiTinh");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("hoTen");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("matKhau");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayTao");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tenDangNhap");
        });

        modelBuilder.Entity<Khuyenmai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KHUYENMA__7A3ECFFF92015FDF");

            entity.ToTable("KHUYENMAI");

            entity.Property(e => e.MaKm)
                .HasMaxLength(50)
                .HasColumnName("maKM");
            entity.Property(e => e.GiaTriGiam)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("giaTriGiam");
            entity.Property(e => e.LoaiKm)
                .HasMaxLength(20)
                .HasColumnName("loaiKM");
            entity.Property(e => e.NgayBatDau)
                .HasColumnType("datetime")
                .HasColumnName("ngayBatDau");
            entity.Property(e => e.NgayKetThuc)
                .HasColumnType("datetime")
                .HasColumnName("ngayKetThuc");
            entity.Property(e => e.TenKhuyenMai)
                .HasMaxLength(150)
                .HasColumnName("tenKhuyenMai");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHANVIEN__7A3EC7D5EF56E1FD");

            entity.ToTable("NHANVIEN");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NHANVIEN__59267D4A14CC9FE5").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NHANVIEN__AB6E6164914EF6C8").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__NHANVIEN__DDDFB4836CE4794A").IsUnique();

            entity.Property(e => e.MaNv)
                .HasMaxLength(50)
                .HasColumnName("maNV");
            entity.Property(e => e.ChucVu)
                .HasMaxLength(50)
                .HasColumnName("chucVu");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("hoTen");
            entity.Property(e => e.Luong)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("luong");
            entity.Property(e => e.MaPhuongNo)
                .HasMaxLength(50)
                .HasColumnName("maPhuongNo");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("matKhau");
            entity.Property(e => e.NgayVaoLam)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("ngayVaoLam");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tenDangNhap");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("DANGLAM")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaPhuongNoNavigation).WithMany(p => p.Nhanvien)
                .HasForeignKey(d => d.MaPhuongNo)
                .HasConstraintName("FK_NV_PX");
        });

        modelBuilder.Entity<Nhaxe>(entity =>
        {
            entity.HasKey(e => e.MaNhaXe).HasName("PK__NHAXE__47CD139504C5EDEE");

            entity.ToTable("NHAXE");

            entity.HasIndex(e => e.Sdt, "UQ__NHAXE__DDDFB483DA8558D2").IsUnique();

            entity.Property(e => e.MaNhaXe)
                .HasMaxLength(50)
                .HasColumnName("maNhaXe");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.MaPhuongNo)
                .HasMaxLength(50)
                .HasColumnName("maPhuongNo");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
            entity.Property(e => e.TenNhaXe)
                .HasMaxLength(150)
                .HasColumnName("tenNhaXe");

            entity.HasOne(d => d.MaPhuongNoNavigation).WithMany(p => p.Nhaxe)
                .HasForeignKey(d => d.MaPhuongNo)
                .HasConstraintName("FK_NHAXE_PHUONG");
        });

        modelBuilder.Entity<Phuongxa>(entity =>
        {
            entity.HasKey(e => e.MaPhuong).HasName("PK__PHUONGXA__DF98DF670C1B8784");

            entity.ToTable("PHUONGXA");

            entity.Property(e => e.MaPhuong)
                .HasMaxLength(50)
                .HasColumnName("maPhuong");
            entity.Property(e => e.MaTinhNo)
                .HasMaxLength(50)
                .HasColumnName("maTinhNo");
            entity.Property(e => e.TenPhuong)
                .HasMaxLength(100)
                .HasColumnName("tenPhuong");

            entity.HasOne(d => d.MaTinhNoNavigation).WithMany(p => p.Phuongxa)
                .HasForeignKey(d => d.MaTinhNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHUONGXA_TINH");
        });

        modelBuilder.Entity<Thanhtoan>(entity =>
        {
            entity.HasKey(e => e.MaTt).HasName("PK__THANHTOA__7A2262475DA1173B");

            entity.ToTable("THANHTOAN");

            entity.HasIndex(e => e.MaDon, "IX_THANHTOAN_DON");

            entity.Property(e => e.MaTt)
                .HasMaxLength(50)
                .HasColumnName("maTT");
            entity.Property(e => e.MaDon)
                .HasMaxLength(50)
                .HasColumnName("maDon");
            entity.Property(e => e.PhuongThuc)
                .HasMaxLength(50)
                .HasColumnName("phuongThuc");
            entity.Property(e => e.SoTien)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("soTien");
            entity.Property(e => e.ThoiGianTt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("thoiGianTT");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("THANHCONG")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.Thanhtoan)
                .HasForeignKey(d => d.MaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THANHTOAN_DON");
        });

        modelBuilder.Entity<Tinhthanh>(entity =>
        {
            entity.HasKey(e => e.MaTinh).HasName("PK__TINHTHAN__135EFA38E15510CB");

            entity.ToTable("TINHTHANH");

            entity.HasIndex(e => e.TenTinh, "UQ__TINHTHAN__CEF7C4DD03013E43").IsUnique();

            entity.Property(e => e.MaTinh)
                .HasMaxLength(50)
                .HasColumnName("maTinh");
            entity.Property(e => e.Img)
                .HasMaxLength(15)
                .HasColumnName("img");
            entity.Property(e => e.TenTinh)
                .HasMaxLength(100)
                .HasColumnName("tenTinh");
        });

        modelBuilder.Entity<Tuyenduong>(entity =>
        {
            entity.HasKey(e => e.MaTuyen).HasName("PK__TUYENDUO__1577011638FA9864");

            entity.ToTable("TUYENDUONG");

            entity.HasIndex(e => new { e.MaBenDi, e.MaBenDen }, "UQ_TUYENDUONG").IsUnique();

            entity.Property(e => e.MaTuyen)
                .HasMaxLength(50)
                .HasColumnName("maTuyen");
            entity.Property(e => e.KhoangCach)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("khoangCach");
            entity.Property(e => e.MaBenDen)
                .HasMaxLength(50)
                .HasColumnName("maBenDen");
            entity.Property(e => e.MaBenDi)
                .HasMaxLength(50)
                .HasColumnName("maBenDi");
            entity.Property(e => e.ThoiGianDuKien).HasColumnName("thoiGianDuKien");

            entity.HasOne(d => d.MaBenDenNavigation).WithMany(p => p.TuyenduongMaBenDenNavigation)
                .HasForeignKey(d => d.MaBenDen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TUYEN_BENDEN");

            entity.HasOne(d => d.MaBenDiNavigation).WithMany(p => p.TuyenduongMaBenDiNavigation)
                .HasForeignKey(d => d.MaBenDi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TUYEN_BENDI");
        });

        modelBuilder.Entity<VChuyenxe>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_CHUYENXE");

            entity.Property(e => e.BenDen)
                .HasMaxLength(150)
                .HasColumnName("benDen");
            entity.Property(e => e.BenDi)
                .HasMaxLength(150)
                .HasColumnName("benDi");
            entity.Property(e => e.GiaVe)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("giaVe");
            entity.Property(e => e.GioDi).HasColumnName("gioDi");
            entity.Property(e => e.ImgXe)
                .HasMaxLength(15)
                .HasColumnName("imgXe");
            entity.Property(e => e.KhoangCach)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("khoangCach");
            entity.Property(e => e.LoaiXe)
                .HasMaxLength(50)
                .HasColumnName("loaiXe");
            entity.Property(e => e.MaBenDen)
                .HasMaxLength(50)
                .HasColumnName("maBenDen");
            entity.Property(e => e.MaBenDi)
                .HasMaxLength(50)
                .HasColumnName("maBenDi");
            entity.Property(e => e.MaChuyen)
                .HasMaxLength(50)
                .HasColumnName("maChuyen");
            entity.Property(e => e.MaTinhDen)
                .HasMaxLength(50)
                .HasColumnName("maTinhDen");
            entity.Property(e => e.MaTinhDi)
                .HasMaxLength(50)
                .HasColumnName("maTinhDi");
            entity.Property(e => e.NgayDi).HasColumnName("ngayDi");
            entity.Property(e => e.PhuongDen)
                .HasMaxLength(100)
                .HasColumnName("phuongDen");
            entity.Property(e => e.PhuongDi)
                .HasMaxLength(100)
                .HasColumnName("phuongDi");
            entity.Property(e => e.SoGheDaDat).HasColumnName("soGheDaDat");
            entity.Property(e => e.SoGheTrong).HasColumnName("soGheTrong");
            entity.Property(e => e.TenNhaXe)
                .HasMaxLength(150)
                .HasColumnName("tenNhaXe");
            entity.Property(e => e.ThoiGianDuKien).HasColumnName("thoiGianDuKien");
            entity.Property(e => e.TinhDen)
                .HasMaxLength(100)
                .HasColumnName("tinhDen");
            entity.Property(e => e.TinhDi)
                .HasMaxLength(100)
                .HasColumnName("tinhDi");
            entity.Property(e => e.TongGheTot).HasColumnName("tongGheTot");
            entity.Property(e => e.TrangThaiChuyen)
                .HasMaxLength(20)
                .HasColumnName("trangThaiChuyen");
        });

        modelBuilder.Entity<VDoanhthu>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_DOANHTHU");

            entity.Property(e => e.Nam).HasColumnName("nam");
            entity.Property(e => e.SoDon).HasColumnName("soDon");
            entity.Property(e => e.Thang).HasColumnName("thang");
            entity.Property(e => e.TongDoanhThu)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("tongDoanhThu");
        });

        modelBuilder.Entity<Xe>(entity =>
        {
            entity.HasKey(e => e.BienSo).HasName("PK__XE__8563D8C7FC6E60F0");

            entity.ToTable("XE");

            entity.Property(e => e.BienSo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("bienSo");
            entity.Property(e => e.HangXe)
                .HasMaxLength(50)
                .HasColumnName("hangXe");
            entity.Property(e => e.Img)
                .HasMaxLength(15)
                .HasColumnName("img");
            entity.Property(e => e.LoaiXe)
                .HasMaxLength(50)
                .HasColumnName("loaiXe");
            entity.Property(e => e.MaNhaXe)
                .HasMaxLength(50)
                .HasColumnName("maNhaXe");
            entity.Property(e => e.NamSx).HasColumnName("namSX");
            entity.Property(e => e.SoTang)
                .HasDefaultValue(1)
                .HasColumnName("soTang");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("SANSANG")
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaNhaXeNavigation).WithMany(p => p.Xe)
                .HasForeignKey(d => d.MaNhaXe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_XE_NHAXE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
