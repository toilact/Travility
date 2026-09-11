# src/

Mã nguồn ứng dụng. Ba project, phụ thuộc một chiều `WinForms → Core → Data`.

```
Travility.Data      EF model (Database First), Repository        [A]
Travility.Core      Entities, Services, 3 Engine, Events         [C, D]
                    └── Engines/  KHÔNG tham chiếu WinForms
Travility.WinForms  Forms, chia thư mục theo module              [A B C D]
                    Auth/ · Map/ · Trip/ · Budget/ · Itinerary/ · Admin/
```

`Travility.Core` không được tham chiếu `System.Windows.Forms` — đây là điều cho
phép engine chạy và test độc lập.

Chi tiết: [`docs/02-system-architecture.md`](../docs/02-system-architecture.md)
