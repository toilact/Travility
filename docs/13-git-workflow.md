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

Nhóm dùng một nhánh làm việc cố định cho mỗi thành viên:

```
develop-<thành viên A> ──●──●──●──●──┐
develop-<thành viên B> ──●──●──●─────┼──► Pull Request ──► main
develop-<thành viên C> ──●──●──●──●──┤                  (chủ dự án duyệt)
develop-<thành viên D> ──●──●────────┘
```

Mỗi thành viên làm việc trên đúng một nhánh có dạng
`develop-<tên-thành-viên>`. Ví dụ:

```
develop-chi
develop-an
develop-binh
develop-dung
```

Tên sau `develop-` dùng chữ thường, không dấu, không khoảng trắng. Nếu nhóm
thống nhất tên khác, dùng đúng tên đã phân công và không tự đổi giữa chừng.

### Quy tắc về `main`

- `main` **luôn phải build được và chạy được**. Không đẩy code hỏng lên `main`.
- **Không ai commit hoặc push thẳng vào `main`.** Mọi thay đổi phải đi từ nhánh
  `develop-<tên-thành-viên>` qua Pull Request.
- Chỉ chủ dự án được duyệt PR vào `main`. Thành viên tạo PR không tự duyệt PR
  của mình.
- Không `git push --force` lên `main`. Không bao giờ. Không có ngoại lệ.

---

## 3. Đặt tên nhánh

Tên nhánh làm việc của thành viên **bắt buộc** có dạng:

```
develop-<tên-thành-viên>
```

Ví dụ:

```
✅ develop-chi
✅ develop-an
❌ feat/map-layer-toggle       (không đúng mô hình của nhóm)
❌ develop/chi                 (sai tiền tố)
❌ develop-Chi                 (chữ hoa)
```

Phạm vi công việc được thể hiện trong commit message và tiêu đề PR, không tạo
nhánh con riêng cho từng tính năng.

---

## 4. Vòng đời một công việc

### Bước 1 — Chuẩn bị nhánh làm việc lần đầu

Mỗi thành viên tạo nhánh của mình một lần từ `main`. Thay `<tên-thành-viên>`
bằng tên đã được nhóm thống nhất:

```bash
git checkout main
git pull --rebase origin main
git checkout -b develop-<tên-thành-viên>
git push -u origin develop-<tên-thành-viên>
```

Sau lần đầu, không tạo nhánh mới cho mỗi công việc. Tiếp tục dùng đúng nhánh
`develop-<tên-thành-viên>` của mình.

### Bước 2 — Làm việc, commit thường xuyên

```bash
git add <những-file-cụ-thể>          # KHÔNG dùng "git add ." một cách máy móc
git status                            # kiểm tra trước khi commit
git commit -m "feat(map): bật tắt lớp khách sạn và nhà hàng"
```

Commit nhỏ, thường xuyên. Một commit = một việc hoàn chỉnh. Đừng dồn cả ngày
vào một commit khổng lồ — khi cần quay lui sẽ không tách ra được.

### Bước 3 — Đồng bộ với `main` mỗi ngày

Trước khi bắt đầu làm việc và trước khi tạo PR, cập nhật nhánh cá nhân với
`main`:

```bash
git checkout develop-<tên-thành-viên>
git pull --rebase origin main
```

Nếu có conflict, tự xử lý trên nhánh cá nhân, chạy kiểm tra lại rồi mới push.
Không giải quyết conflict trực tiếp trên `main`.

### Bước 4 — Đẩy nhánh cá nhân lên

```bash
git push origin develop-<tên-thành-viên>
```

Nếu đã rebase sau khi push, dùng:

```bash
git push --force-with-lease origin develop-<tên-thành-viên>
```

> `--force-with-lease` an toàn hơn `--force`: nó từ chối đẩy nếu có người khác
> vừa push lên nhánh đó. Chỉ dùng trên nhánh `develop-<tên-thành-viên>` của
> chính mình. Không bao giờ dùng trên `main`.

### Bước 5 — Mở Pull Request vào `main`

```bash
gh pr create --base main --head develop-<tên-thành-viên> --fill
# hoặc mở https://github.com/toilact/Travility/pulls
```

PR phải có:

- Base branch: `main`.
- Head branch: `develop-<tên-thành-viên>` của người tạo.
- Tiêu đề theo mẫu commit, ví dụ `feat(map): bật tắt lớp địa điểm`.
- Mô tả nêu rõ làm gì, vì sao và đã kiểm tra thế nào.
- Checklist trong PR template đã được tick đúng với những gì thực sự kiểm tra.

### Bước 6 — Chờ chủ dự án duyệt

Chủ dự án là người duyệt PR cuối cùng. Thành viên cần phản hồi comment review,
cập nhật code trên chính nhánh `develop-<tên-thành-viên>` và chờ duyệt lại.

Chỉ merge sau khi chủ dự án đã approve. Người tạo PR không tự merge PR của mình.

### Bước 7 — Merge và đồng bộ lại

Sau khi chủ dự án đã approve, chủ dự án merge PR vào `main` bằng **Squash and
merge**, rồi xoá PR branch nếu GitHub đề nghị. Thành viên cập nhật nhánh cá nhân:

```bash
git checkout develop-<tên-thành-viên>
git pull --rebase origin main
```

Nhánh `develop-<tên-thành-viên>` là nhánh làm việc lâu dài, vì vậy không xoá
nhánh này sau mỗi PR.

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

PR luôn đi từ `develop-<tên-thành-viên>` vào `main`. Không tạo PR ngược chiều
`main` → nhánh cá nhân để đưa code vào dự án.

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

**Chủ dự án phải approve trước khi merge.** Người tạo PR không tự approve hoặc
tự merge PR của mình.

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

Không xoá nhánh `develop-<tên-thành-viên>` sau khi merge vì đây là nhánh làm
việc lâu dài của thành viên. Chỉ xoá các nhánh tạm nếu nhóm có tạo nhánh tạm để
xử lý sự cố.

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
    <Compile Include="Map\ExploreMapForm.cs" />
    <Compile Include="Budget\BudgetForm.cs" />
```

Git có thể hiển thị hai phần thay đổi bằng các dấu đánh dấu xung đột. **Cách xử
lý: giữ CẢ HAI dòng**, xoá các dấu đánh dấu xung đột rồi kiểm tra lại file:

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

## 8. Bảo vệ nhánh `main` (chủ dự án thiết lập)

Vào **Settings → Branches → Add branch protection rule**, nhánh `main`:

- [x] Require a pull request before merging
- [x] Require approvals: **1**
- [x] Require branches to be up to date before merging
- [ ] ~~Require status checks~~ *(chưa có CI, bỏ qua)*

Lý do bật: nó biến ba luật ở mục 0 từ *lời hứa* thành *ràng buộc kỹ thuật*.
Không ai vô tình push thẳng lên `main` được nữa. Nếu GitHub có tuỳ chọn
**Restrict who can push to matching branches**, chỉ cho chủ dự án push/merge
vào `main`.

---

## 9. Những điều cấm tuyệt đối

```
❌ git push --force origin main
❌ git reset --hard khi chưa commit việc đang làm
❌ Commit API key, mật khẩu, connection string có mật khẩu thật
❌ Commit bin/ obj/ packages/ .vs/
❌ Sửa file của người khác mà không báo
❌ Merge PR của chính mình khi chưa được chủ dự án approve
❌ Đẩy code không build được lên main
```

Về connection string: để trong `App.config` với giá trị local (`.\SQLEXPRESS`,
Windows Authentication). Nếu cần mật khẩu thật, tách ra file riêng và cho vào
`.gitignore`.

---

## 10. Cheat sheet

```bash
# Chuẩn bị nhánh lần đầu
git checkout main && git pull --rebase origin main
git checkout -b develop-<tên-thành-viên>
git push -u origin develop-<tên-thành-viên>

# Trong lúc làm
git status                         # xem đang có gì
git diff                           # xem mình đã sửa gì
git add <file> && git commit -m "feat(scope): mô tả"
git pull --rebase origin main      # đồng bộ, làm hằng ngày

# Đẩy nhánh cá nhân và mở PR vào main
git push origin develop-<tên-thành-viên>
gh pr create --base main --head develop-<tên-thành-viên> --fill

# Sau khi chủ dự án merge PR
git checkout develop-<tên-thành-viên>
git pull --rebase origin main

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
