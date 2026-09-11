# 11 — Bảo mật

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md)
> **Sở hữu:** A
> **Đây là mục rubric hay có mà đặc tả gốc bỏ quên.**

---

## 1. Mật khẩu — bắt buộc hash

Lưu plaintext là lỗi bị trừ điểm chắc chắn, và là câu hỏi hội đồng hay hỏi nhất
về bảo mật.

Dùng **PBKDF2** có sẵn trong .NET Framework, không cần thư viện ngoài:

```csharp
// Đăng ký
var salt = new byte[16];
using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);

using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
{
    byte[] hash = pbkdf2.GetBytes(32);
    // Lưu: PasswordHash (32 byte) + PasswordSalt (16 byte) + Iterations (10000)
}
```

Bảng `Users` lưu `PasswordHash`, `PasswordSalt`, `Iterations` — **không** có cột
`Password`.

Khi so sánh hash, dùng so sánh **thời gian hằng số**, không dùng `==` trên chuỗi.

### Quy tắc mật khẩu tối thiểu

Tối thiểu 8 ký tự. Không ép thêm quy tắc phức tạp — với đồ án nó chỉ làm demo
khó chịu.

---

## 2. API key và bí mật

**Không hardcode key trong source.** Nếu nhóm đẩy key lên GitHub public, các bot
quét sẽ tìm thấy và nhà cung cấp vô hiệu hoá nó trong vài giờ.

```
config/api-keys.json            ← key thật, NẰM TRONG .gitignore
config/api-keys.example.json    ← file mẫu, commit, không có key thật
```

```csharp
var cfg = JsonConvert.DeserializeObject<ApiKeys>(File.ReadAllText("config/api-keys.json"));
```

Nếu file không tồn tại, hiện thông báo hướng dẫn copy từ `.example` — không crash.

---

## 3. Connection string

Để trong `App.config` với giá trị local:

```xml
<connectionStrings>
  <add name="TravilityDb"
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=Travility;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Dùng **Windows Authentication** (`Integrated Security=True`), không nhúng mật
khẩu SQL. Nếu bắt buộc dùng SQL Authentication thì tách connection string ra file
riêng nằm ngoài git.

---

## 4. Chống SQL Injection

Mọi truy vấn ADO.NET dùng **tham số**, không nối chuỗi:

```csharp
// ✅
cmd.CommandText = "SELECT * FROM Places WHERE CategoryId = @catId";
cmd.Parameters.AddWithValue("@catId", categoryId);

// ❌ không bao giờ
cmd.CommandText = "SELECT * FROM Places WHERE CategoryId = " + categoryId;
```

EF đã tham số hoá sẵn, nhưng `SqlQuery` thô thì không — vẫn phải tham số.

---

## 5. Phân quyền

Hai vai trò: `Admin` và `Traveler`.

- Kiểm tra vai trò ở **tầng Service**, không chỉ ẩn nút trên giao diện. Ẩn nút là
  trang trí, không phải bảo mật.
- Form Admin kiểm tra vai trò trong `Load`, thoát nếu không đủ quyền.
- Người dùng chỉ đọc/sửa được `Trip`, `Expense`, `CheckIn` **của chính mình** —
  lọc theo `UserId` ở tầng Repository.

---

## 6. Dữ liệu cá nhân

`CheckIn` lưu toạ độ thật của người dùng. Trong phạm vi đồ án điều này chấp nhận
được, nhưng nên nêu trong báo cáo rằng dữ liệu vị trí là dữ liệu nhạy cảm, và cờ
`IsSimulated` cho biết bản ghi nào là thật.

Không hiển thị vị trí của người dùng này cho người dùng khác.

---

## 7. Bẫy đã biết

| Bẫy | Cách tránh |
|---|---|
| "Lưu plaintext để test cho nhanh" rồi quên sửa | Làm hash ngay từ đầu |
| Hardcode API key rồi đẩy lên GitHub | File config ngoài git + file `.example` |
| Nối chuỗi SQL | Luôn dùng tham số |
| Chỉ ẩn nút Admin trên UI | Kiểm tra vai trò ở tầng Service |
| Quên lọc theo `UserId` | Người này xem được chuyến của người kia |
| Commit `config/api-keys.json` | Kiểm tra `.gitignore` trước commit đầu tiên |
