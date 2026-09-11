# database/

```
schema.sql            Định nghĩa bảng, khoá, index, stored procedure
seed_categories.sql   Danh mục địa điểm, nhóm ngân sách, huy hiệu
seed_places.sql       120–150 Place khu vực Đà Nẵng
seed_transport.sql    TransportOptions (dữ liệu mô phỏng)
seed_demo.sql         Tài khoản demo có sẵn Trip và Achievement
```

## Quy tắc

- **Chỉ A được sửa `schema.sql`.** Ai cần thêm bảng/cột thì báo A.
- Mọi seed là file `.sql` trong git — không seed tay trong SSMS rồi quên.
- Đổi schema: báo cả nhóm → regenerate EF model → merge ngay.

Hợp đồng trường bắt buộc của bảng `Places`:
[`docs/03-database-design.md`](../docs/03-database-design.md) mục 3.
