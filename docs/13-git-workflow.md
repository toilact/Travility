# 13 — Quy tắc Git: nhánh, commit, Pull Request

> **Repo:** https://github.com/toilact/Travility
> **Nhánh chính:** `main`
> **Áp dụng cho:** cả 4 thành viên, từ ngày đầu tiên
> **Đọc kèm:** [`00-project-overview.md`](./00-project-overview.md)

---

## 0. Ba luật không được phá

Ba điều này gây ra gần như toàn bộ tai nạn git của đồ án nhóm. Đọc trước, nhớ kỹ.

| # | Luật | Vì sao |
|---|---|---|
| 1 | **Không bao giờ commit `bin/`, `obj/`, `packages/`, `.vs/`** | Mỗi lần build sinh hàng trăm file nhị phân. Commit chúng = conflict liên miên, repo phình, và không ai gỡ được |
| 2 | **Không ai mở Form của người khác trong Visual Studio Designer** | `.Designer.cs` là file sinh tự động. Hai người cùng mở = merge hỏng, phải dựng lại form bằng tay |
| 3 | **Conflict trên `.csproj`: GIỮ CẢ HAI dòng** `<Compile Include=...>` | Xoá dòng của người khác = xoá file của họ khỏi project. Họ sẽ không hiểu vì sao code biến mất |

---

## 1. Thiết lập lần đầu (mỗi người làm một lần)

```bash
# Clone repo
git clone https://github.com/toilact/Travility.git
cd Travility

# Khai báo danh tính - PHẢI làm, nếu không commit sẽ không ghi nhận đúng người
git config user.name  "Tên của bạn"
git config user.email "email@cua.ban"

# Bật rebase mặc định khi pull -> lịch sử sạch, không đầy merge commit rác
git config pull.rebase true

# Giữ nguyên kiểu xuống dòng (tránh toàn bộ file bị đánh dấu thay đổi)
git config core.autocrlf true
```

**Kiểm tra `.gitignore` đã có trước khi commit lần đầu.** Nếu chưa có, dừng lại
và báo A. Đây là việc số 1 của ngày 1.

---

## 2. Mô hình nhánh

Nhóm dùng **trunk-based development với nhánh tính năng ngắn hạn**:

```
main  ──●────●────●────●────●────●──►   luôn build được, luôn chạy được
         \        /      \      /
          ●──●──●         ●──●──●       nhánh tính năng, sống ngắn
```

**Không dùng nhánh `develop`.** Mô hình Git Flow (main + develop + release +
hotfix) sinh ra để quản lý nhiều phiên bản phát hành song song — đồ án 6 tuần
không có bài toán đó, thêm tầng chỉ tốn công điều hướng.

### Quy tắc về `main`

- `main` **luôn phải build được và chạy được**. Không đẩy code hỏng lên `main`.
- **Không ai commit thẳng vào `main`.** Mọi thay đổi đi qua Pull Request.
- Không `git push --force` lên `main`. Không bao giờ. Không có ngoại lệ.

---

## 3. Đặt tên nhánh

```
<loại>/<mô-tả-ngắn-bằng-gạch-nối>
```

| Loại | Dùng khi | Ví dụ |
|---|---|---|
| `feat/` | Thêm tính năng mới | `feat/map-layer-toggle` |
| `fix/` | Sửa lỗi | `fix/budget-pricing-unit` |
| `refactor/` | Sửa cấu trúc, không đổi hành vi | `refactor/split-trip-service` |
| `test/` | Thêm hoặc sửa unit test | `test/itinerary-budget-constraint` |
| `docs/` | Tài liệu | `docs/erd-data-dictionary` |
| `chore/` | Cấu hình, thư viện, việc vặt | `chore/add-newtonsoft-json` |

**Quy tắc:** tiếng Anh, chữ thường, gạch nối, không dấu tiếng Việt, không khoảng
trắng. Tên phải nói được nhánh làm gì mà không cần mở ra xem.

```
✅ feat/checkin-simulated-location
✅ fix/haversine-detour-factor
❌ feat/sua-loi              (mơ hồ)
❌ B                          (không ai biết là gì)
❌ feat/Map Layer             (có khoảng trắng và chữ hoa)
```

---

## 4. Vòng đời một công việc

### Bước 1 — Cập nhật `main` rồi tạo nhánh

```bash
git checkout main
git pull --rebase origin main        # LUÔN pull trước khi tạo nhánh mới
git checkout -b feat/map-layer-toggle
```

### Bước 2 — Làm việc, commit thường xuyên

```bash
git add <những-file-cụ-thể>          # KHÔNG dùng "git add ." một cách máy móc
git status                            # kiểm tra trước khi commit
git commit -m "feat(map): bật/tắt lớp khách sạn và nhà hàng"
```

Commit nhỏ, thường xuyên. Một commit = một việc hoàn chỉnh. Đừng dồn cả ngày
vào một commit khổng lồ — khi cần quay lui sẽ không tách ra được.

### Bước 3 — Đồng bộ với `main` mỗi ngày

```bash
git pull --rebase origin main
```

Làm **hằng ngày**, kể cả khi nhánh chưa xong. Rebase mỗi ngày giải quyết 2–3
conflict nhỏ; để một tuần sẽ phải giải quyết 30 conflict cùng lúc.

### Bước 4 — Đẩy nhánh lên

```bash
git push -u origin feat/map-layer-toggle     # lần đầu
git push                                      # các lần sau
```

Nếu đã rebase sau khi push, dùng:

```bash
git push --force-with-lease                   # CHỈ trên nhánh của chính mình
```

> `--force-with-lease` an toàn hơn `--force`: nó từ chối đẩy nếu có người khác
> vừa push lên nhánh đó. Không bao giờ dùng cả hai trên `main`.

### Bước 5 — Mở Pull Request

```bash
gh pr create --fill
# hoặc mở https://github.com/toilact/Travility/pulls
```

### Bước 6 — Merge và dọn dẹp

```bash
gh pr merge --squash --delete-branch
git checkout main
git pull --rebase origin main
```

---

## 5. Quy tắc viết commit message

Dùng **Conventional Commits** rút gọn:

```
<loại>(<phạm vi>): <mô tả ngắn, tiếng Việt được>

[thân commit nếu cần giải thích VÌ SAO]
```

Loại: `feat` · `fix` · `refactor` · `test` · `docs` · `chore`
Phạm vi: `map` · `trip` · `budget` · `engine` · `auth` · `admin` · `chat` · `db`

```
✅ feat(engine): thêm tầng chia ngày theo cụm địa lý
✅ fix(budget): khách sạn tính theo phòng thay vì theo người
✅ test(engine): kiểm tra lịch trình không vượt ngân sách
✅ docs: bổ sung ERD và data dictionary

❌ update                      (update cái gì?)
❌ fix bug                     (bug nào?)
❌ asdfgh                      (…)
❌ sửa lần cuối                (không bao giờ là lần cuối)
```

**Mô tả nên nói cái gì thay đổi, thân commit nói vì sao.** Sáu tuần nữa khi cần
tìm lại một quyết định, thân commit là thứ cứu bạn.

---

## 6. Pull Request

### Khi nào mở PR

Khi một công việc **hoàn chỉnh và build được**. Không mở PR cho code đang dở —
nếu muốn cho nhóm xem sớm, mở **Draft PR**.

### Nội dung PR

Tiêu đề viết như commit message. Phần mô tả trả lời ba câu:

```markdown
## Làm gì
Thêm bật/tắt lớp marker trên Smart Map (Khách sạn, Nhà hàng, Lịch sử...).

## Vì sao
Đặc tả §4 — tránh hiển thị quá nhiều marker cùng lúc.

## Đã kiểm tra thế nào
- Bật/tắt từng lớp, marker ẩn/hiện đúng
- Ngắt mạng, bản đồ vẫn hiện (tile đã cache)
- Build Release chạy được
```

### Ai review

**Khuyến nghị: 1 người approve trước khi merge.**

Nhưng để review không trở thành nút thắt trong 6 tuần:

> **Nếu sau 12 giờ không ai review, người tạo PR được tự merge.**

Đây là dòng duy nhất trong tài liệu này mà nhóm nên cân nhắc đổi theo thực tế.
Chặt hơn thì bỏ điều khoản 12 giờ; lỏng hơn thì cho tự merge ngay.

Ai review PR của ai:

```
A ←→ D        (nền móng ←→ engine)
B ←→ C        (bản đồ ←→ nghiệp vụ)
```

Ghép chéo như vậy để mỗi người hiểu được ít nhất một module không phải của mình —
có ích khi hội đồng hỏi về phần người khác làm.

### Cách merge

**Luôn dùng Squash and merge.** Toàn bộ commit của nhánh gộp thành một commit
trên `main`. Lịch sử `main` sạch, mỗi dòng là một tính năng hoàn chỉnh, dễ đọc
khi làm báo cáo và dễ quay lui khi hỏng.

Xoá nhánh ngay sau khi merge.

### Review như thế nào

Review không phải để bắt lỗi chính tả. Ba câu cần trả lời:

1. Code có làm đúng cái mô tả PR nói không?
2. Có phá vỡ interface chung trong `Travility.Core` không?
3. Có vi phạm ba luật ở mục 0 không?

Nếu ổn thì approve. Đừng giữ PR lại vì khác phong cách đặt tên biến.

---

## 7. Xử lý conflict

### `.csproj` — hay gặp nhất

Xảy ra khi hai người cùng thêm file mới. Mở ra sẽ thấy:

```xml
<<<<<<< HEAD
    <Compile Include="Map\ExploreMapForm.cs" />
=======
    <Compile Include="Budget\BudgetForm.cs" />
>>>>>>> feat/budget-dashboard
```

**Cách xử lý: giữ CẢ HAI dòng**, xoá ba dòng đánh dấu:

```xml
    <Compile Include="Map\ExploreMapForm.cs" />
    <Compile Include="Budget\BudgetForm.cs" />
```

### `.Designer.cs` — đừng để xảy ra

Nếu đã xảy ra thì tức là luật số 2 đã bị phá. Không có cách merge an toàn cho
file này. Cách xử lý:

```bash
git checkout --theirs <đường-dẫn-file>    # lấy bản của người kia
```

rồi **người phá luật tự dựng lại phần của mình bằng Designer**. Đây là lý do
luật số 2 tồn tại.

### `schema.sql` — chỉ A sửa

Nếu bạn không phải A mà file này conflict, tức là bạn đã sửa nhầm. Lấy bản của
`main` và báo A:

```bash
git checkout --theirs database/schema.sql
```

### Rebase đang dở, muốn thoát

```bash
git rebase --abort        # huỷ, quay về trạng thái trước khi rebase
```

Luôn có đường lui. Đừng hoảng.

---

## 8. Bảo vệ nhánh `main` (A thiết lập, 2 phút)

Vào **Settings → Branches → Add branch protection rule**, nhánh `main`:

- [x] Require a pull request before merging
- [x] Require approvals: **1**
- [x] Require branches to be up to date before merging
- [ ] ~~Require status checks~~ *(chưa có CI, bỏ qua)*

Lý do bật: nó biến ba luật ở mục 0 từ *lời hứa* thành *ràng buộc kỹ thuật*.
Không ai vô tình push thẳng lên `main` được nữa.

---

## 9. Những điều cấm tuyệt đối

```
❌ git push --force origin main
❌ git reset --hard khi chưa commit việc đang làm
❌ Commit API key, mật khẩu, connection string có mật khẩu thật
❌ Commit bin/ obj/ packages/ .vs/
❌ Sửa file của người khác mà không báo
❌ Merge PR của chính mình khi chưa đủ 12h và chưa ai xem
❌ Đẩy code không build được lên main
```

Về connection string: để trong `App.config` với giá trị local (`.\SQLEXPRESS`,
Windows Authentication). Nếu cần mật khẩu thật, tách ra file riêng và cho vào
`.gitignore`.

---

## 10. Cheat sheet

```bash
# Bắt đầu việc mới
git checkout main && git pull --rebase origin main
git checkout -b feat/ten-tinh-nang

# Trong lúc làm
git status                         # xem đang có gì
git diff                           # xem mình đã sửa gì
git add <file> && git commit -m "feat(scope): mô tả"
git pull --rebase origin main      # đồng bộ, làm hằng ngày

# Đẩy lên và mở PR
git push -u origin feat/ten-tinh-nang
gh pr create --fill

# Sau khi PR được duyệt
gh pr merge --squash --delete-branch
git checkout main && git pull --rebase origin main

# Xem tình hình
git log --oneline --graph --all -20
gh pr list
gh pr status
```

---

## 11. Sự cố thường gặp

| Tình huống | Cách xử lý |
|---|---|
| Lỡ commit `bin/`, `obj/` rồi | `git rm -r --cached bin obj` → sửa `.gitignore` → commit lại |
| Commit nhầm nhánh `main` (chưa push) | `git reset --soft HEAD~1` → tạo nhánh → commit lại |
| Commit message sai (chưa push) | `git commit --amend -m "message đúng"` |
| Muốn bỏ hết thay đổi chưa commit | `git restore .` — **cẩn thận, không lấy lại được** |
| Cất tạm việc đang làm để chuyển nhánh | `git stash` → làm việc khác → `git stash pop` |
| Rebase rối quá | `git rebase --abort`, hỏi nhóm, đừng tự mò |
| Xoá nhầm nhánh chưa merge | `git reflog` tìm commit cuối → `git checkout -b ten-nhanh <hash>` |
| Kéo về bị lỗi "divergent branches" | `git pull --rebase origin main` |

**Nguyên tắc chung khi hoảng:** commit đã tạo gần như không bao giờ mất thật.
`git reflog` lưu lại mọi thứ trong 90 ngày. Đừng xoá thư mục đi clone lại.
