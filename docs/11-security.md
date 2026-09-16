# 11 — Bảo mật

> **Đọc trước:** [`CONTEXT.md`](../CONTEXT.md)
> **Sở hữu:** A
> **Đây là mục rubric hay có mà đặc tả gốc bỏ quên.**

---

## 1. Mật khẩu — bắt buộc hash

Lưu plaintext là lỗi bị trừ điểm chắc chắn, và là câu hỏi hội đồng hay hỏi nhất
về bảo mật.

Dùng **PBKDF2-HMAC-SHA256** có sẵn trong .NET Framework, thông qua
`Travility.Core.Security.IPasswordHasher`:

```csharp
var hasher = new Pbkdf2PasswordHasher(600000);
PasswordHash stored = hasher.Hash(password);
bool matches = hasher.Verify(candidatePassword, stored);
```

Baseline **600.000 vòng**, salt CSPRNG riêng **16 byte**, derived key **32 byte**.
Bảng `Users` lưu `PasswordHash`, `PasswordSalt`, `PasswordIterations` và
`PasswordAlgorithm` — **không** có cột `Password`. `Verify` sử dụng iteration
đã lưu theo user để hash cũ có iteration thấp vẫn xác minh được.

Khi so sánh hash, duyệt đủ **32 byte** bằng XOR/OR, không thoát sớm khi gặp byte
khác nhau. `PasswordHash` sao chép mảng đầu vào và đầu ra để giữ bất biến.
`Verify` trả `false` nếu password không hợp lệ, hash/salt null hoặc sai độ dài,
algorithm không khớp chính xác hay iteration không dương.

Trước khi đóng băng cấu hình, benchmark trên **laptop yếu nhất nhóm**, mục tiêu
mỗi lần hash/verify dưới khoảng một giây. Chỉ cân nhắc giảm iteration nếu vượt
mốc đó và ghi lại kết quả; build/test trên CI không thay thế benchmark máy demo.

### Quy tắc mật khẩu tối thiểu

Từ **8 đến 128 ký tự** (theo `string.Length` của .NET), không trim hoặc normalize
password trước khi hash. `Hash` từ chối input ngoài khoảng này bằng
`ArgumentException`; Service phải validate trước và trả lỗi nghiệp vụ phù hợp
cho UI. Không ép thêm quy tắc phức tạp.

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
