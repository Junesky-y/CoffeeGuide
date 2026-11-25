# Зображення

Кожен запис у JSON уже має шлях `/images/<папка>/<slug>.jpg`. Щоб замість заглушки з'явилося фото,
достатньо **покласти файл із точно такою назвою** в потрібну папку й перезапустити застосунок.
Нічого змінювати в коді чи JSON не треба.

## Правила
- Формат: `.jpg` (також підходять `.jpeg`, `.png`, `.webp` із тією самою назвою).
- Пропорції 4:3 (наприклад, 1200×900 px). Інші пропорції теж працюватимуть: фото обрізається по центру.
- Розмір до 200–300 КБ. Стиснути можна на https://squoosh.app.
- Назва файлу = `slug` латиницею малими літерами, як у таблицях нижче.
- Використовуйте лише фото з вільною ліцензією (Unsplash, Pexels, Wikimedia Commons) або власні. Запишіть автора й посилання
  у файл `wwwroot/images/CREDITS.md`.

## Додаткові (необов'язкові) зображення
- `wwwroot/images/hero.jpg` — фон головного банера (горизонтальне фото, від 1600 px завширшки).
- `wwwroot/images/categories/beans.jpg`, `milk.jpg`, `syrups.jpg`, `methods.jpg`, `recipes.jpg` — фото на плитках головної.

Якщо файлу немає, показується акуратна заглушка; у консолі при запуску буде одне повідомлення з кількістю відсутніх фото.

## Кавові зерна — `wwwroot/images/beans/`

| Файл | Що зображено |
|---|---|
| `ethiopia-yirgacheffe.jpg` | Ефіопія Іргачеф |
| `colombia-huila.jpg` | Колумбія Уїла |
| `brazil-santos.jpg` | Бразилія Сантос |
| `kenya-aa.jpg` | Кенія AA |
| `vietnam-robusta.jpg` | В'єтнам Робуста |
| `espresso-blend.jpg` | Бленд для еспресо |
| `guatemala-antigua.jpg` | Гватемала Антігуа |
| `costa-rica-tarrazu.jpg` | Коста-Рика Таррасу |
| `sumatra-mandheling.jpg` | Індонезія Суматра Мандхелінг |
| `india-monsooned-malabar.jpg` | Індія Мусонний Малабар |
| `philippines-liberica.jpg` | Філіппіни Ліберіка (Барако) |

## Молоко — `wwwroot/images/milk/`

| Файл | Що зображено |
|---|---|
| `whole-milk.jpg` | Цільне коров'яче молоко |
| `skim-milk.jpg` | Знежирене молоко |
| `oat-milk.jpg` | Вівсяне молоко |
| `soy-milk.jpg` | Соєве молоко |
| `almond-milk.jpg` | Мигдалеве молоко |
| `coconut-milk.jpg` | Кокосове молоко |
| `lactose-free-milk.jpg` | Безлактозне молоко |

## Сиропи — `wwwroot/images/syrups/`

| Файл | Що зображено |
|---|---|
| `vanilla-syrup.jpg` | Ванільний сироп |
| `caramel-syrup.jpg` | Карамельний сироп |
| `hazelnut-syrup.jpg` | Горіховий сироп (фундук) |
| `cinnamon-syrup.jpg` | Сироп із корицею |
| `raspberry-syrup.jpg` | Малиновий сироп |
| `chocolate-syrup.jpg` | Шоколадний сироп |
| `coconut-syrup.jpg` | Кокосовий сироп |

## Способи приготування — `wwwroot/images/brewing/`

| Файл | Що зображено |
|---|---|
| `espresso-machine.jpg` | Еспресо-машина |
| `pour-over.jpg` | Пуровер (V60) |
| `french-press.jpg` | Френч-прес |
| `aeropress.jpg` | Аеропрес |
| `moka-pot.jpg` | Гейзерна кавоварка (мока) |
| `cezve.jpg` | Турка (джезва) |
| `cold-brew.jpg` | Холодне заварювання (cold brew) |

## Рецепти — `wwwroot/images/recipes/`

| Файл | Що зображено |
|---|---|
| `espresso.jpg` | Еспресо |
| `americano.jpg` | Американо |
| `cappuccino.jpg` | Капучино |
| `latte.jpg` | Лате |
| `iced-latte.jpg` | Айс-лате |
| `mocha.jpg` | Мокка |
| `affogato.jpg` | Афогато |
| `flat-white.jpg` | Флет вайт |
| `macchiato.jpg` | Еспресо макіато |
| `cortado.jpg` | Кортадо |
| `raf.jpg` | Раф |
| `turkish-coffee.jpg` | Кава по-турецьки |
| `cold-brew.jpg` | Колд брю |
| `espresso-tonic.jpg` | Еспресо-тонік |
