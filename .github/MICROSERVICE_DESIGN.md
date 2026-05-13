# מיקרו־שירותים ל־Improved-version-of-Chinese-sales-system

## מבוא
מסמך זה מציע ארכיטקטורה מיקרו־שירותית ל־פרויקט הקיים, עם Boundaries, Service responsibilities, Recommended databases, Table schemas, Key entities, ו־Integration patterns.

---

## גבולות מיקרו־שירותים (Microservice boundaries)

1. User Service
	- `UserController`, `AddressController`
	- ניהול חיבור משתמשים, auth, פרופיל, כתובות

2. Catalog Service
	- `GiftController`, `PackageController`, `CategoryController`
	- ניהול קטלוג מוצרים, חבילות, קטגוריות, זמינות ומחירים

3. Commerce Service
	- `CardCartController`, `PackageCartController`, `CardController`
	- סלים, רכישות, ניהול קניות בפועל

4. Lottery Service
	- `LotteryController`
	- ניהול לוטו, מחזורי הגרלות, סטטוס, סיום, חישוב סכומים

5. Donor Service
	- `DonorController`
	- ניהול תורמים, חברות, כתובות חברה, מתנות קשורות

6. File Service
	- `FilesController`
	- ניהול תמונות וקבצים, Media storage, Upload/download

7. API Gateway / BFF
	- תיווך בין Angular frontend לשירותים, Routing, Authorization header, Rate limiting

---

## שירותים ואחריות (Service responsibilities)

### User Service
- רישום משתמשים
- התחברות/אימות
- עדכון פרופיל
- ניהול `Address`
- הרשאות `IsAdmin`

### Catalog Service
- יצירת/עדכון `Gift`, `Package`, `Category`
- ניהול `ImageUrl`, `Price`, `GiftValue`, `IsPackageAble`
- חיפוש לפי קטגוריה, תורם, לוטו

### Commerce Service
- ניהול סל קניות של `CardCart` ו־`PackageCart`
- ביצוע רכישות
- ניהול כרטיסים (`Card`) וסטטוס `IsWin`
- חיבור בין משתמשים למוצרים שנרכשו

### Lottery Service
- ניהול מאגרי לוטו
- פתיחה וסגירה של מחזורים
- חישוב סטטיסטיקות `TotalCards`, `TotalSum`
- סימון `IsDone`

### Donor Service
- פרטי תורם וכתובת חברה
- קישור תורם ל־`Gift` ו־`Lottery`
- ניהול משלוחים ושיתופי פעולה

### File Service
- אחסון תמונות ל־`Gift`, `Package`, `Donor`
- שימוש ב־object storage (S3 / Azure Blob)
- זמני life-cycle וקבצי metadata

---

## מסד נתונים מומלץ לכל שירות (Recommended database per service)

| Service | Recommended DB | Notes |
|---|---|---|
| User Service | SQL Server / PostgreSQL | transactional, identity, relationships |
| Catalog Service | SQL Server / PostgreSQL | product catalog, metadata |
| Commerce Service | SQL Server / PostgreSQL | cart, orders, transactions |
| Lottery Service | SQL Server / PostgreSQL | event-driven status, analytics |
| Donor Service | SQL Server / PostgreSQL | CRM-like data |
| File Service | Blob Storage + relational metadata DB | file metadata in DB, binary in S3/Azure Blob |

> Database-per-service מבודד את הטרנזקציות ומאפשר סקיילינג עצמאי.

---

## סכימות טבלאיות (Table schemas)

### User Service
- `Users`
  - `Id` int PK
  - `UserName` nvarchar(30) NOT NULL
  - `Password` nvarchar(15) NOT NULL
  - `FirstName` nvarchar(30)
  - `LastName` nvarchar(30) NOT NULL
  - `Email` nvarchar(256) NOT NULL
  - `Phone` nvarchar(20)
  - `IsAdmin` bit NOT NULL DEFAULT 0
  - `AddressId` int FK

- `Addresses`
  - `Id` int PK
  - `City` nvarchar(50) NOT NULL
  - `Street` nvarchar(50) NOT NULL
  - `Number` int
  - `ZipCode` int
  - `UserId` int FK NULL
  - `DonorId` int FK NULL

### Catalog Service
- `Gifts`
  - `Id` int PK
  - `Name` nvarchar(50) NOT NULL
  - `Description` nvarchar(250)
  - `Price` int
  - `GiftValue` int
  - `ImageUrl` nvarchar(250)
  - `IsPackageAble` bit DEFAULT 1
  - `DonorId` int FK
  - `CategoryId` int FK NULL
  - `LotteryId` int FK

- `Packages`
  - `Id` int PK
  - `Name` nvarchar(50) NOT NULL
  - `Description` nvarchar(250)
  - `ImageUrl` nvarchar(500)
  - `NumOfCards` int NOT NULL
  - `Price` int NOT NULL
  - `LotteryId` int FK NOT NULL

- `Categories`
  - `Id` int PK
  - `Name` nvarchar(50) NOT NULL
  - `Description` nvarchar(250)

### Commerce Service
- `CardCarts`
  - `Id` int PK
  - `Quantity` int NOT NULL DEFAULT 1
  - `UserId` int FK NOT NULL
  - `GiftId` int FK NOT NULL

- `PackageCarts`
  - `Id` int PK
  - `Quantity` int NOT NULL DEFAULT 1
  - `UserId` int FK NOT NULL
  - `PackageId` int FK NOT NULL

- `Cards`
  - `Id` int PK
  - `IsWin` bit DEFAULT 0
  - `UserId` int FK NOT NULL
  - `GiftId` int FK NOT NULL

### Lottery Service
- `Lotteries`
  - `Id` int PK
  - `Name` nvarchar(50) NOT NULL
  - `StartDate` datetime NOT NULL
  - `EndDate` datetime NOT NULL
  - `TotalCards` int
  - `TotalSum` int
  - `IsDone` bit NOT NULL DEFAULT 0

### Donor Service
- `Donors`
  - `Id` int PK
  - `FirstName` nvarchar(30)
  - `LastName` nvarchar(30)
  - `CompanyName` nvarchar(30) NOT NULL
  - `CompanyEmail` nvarchar(256) NOT NULL
  - `CompanyPhone` nvarchar(20)
  - `CompanyIcon` nvarchar(250)
  - `CompanyAddressId` int FK NOT NULL

### File Service
- `FileMetadata`
  - `Id` GUID PK
  - `EntityType` nvarchar(50)
  - `EntityId` int
  - `FileName` nvarchar(250)
  - `ContentType` nvarchar(100)
  - `Url` nvarchar(500)
  - `CreatedAt` datetime
  - `StorageProvider` nvarchar(50)

---

## ישויות מפתח (Key entities)

- `User`
- `Address`
- `Gift`
- `Package`
- `Category`
- `CardCart`
- `PackageCart`
- `Card`
- `Lottery`
- `Donor`
- `FileMetadata`

---

## דפוסי אינטגרציה (Integration patterns)

1. API Gateway
	- כל קריאת frontend עוברת ל־Gateway
	- Gateway מאמתת JWT, מוסיפה headers, מפזרת requests לשירותים שונים

2. REST synchronous communication
	- Frontend ↔ User Service
	- Frontend ↔ Catalog Service
	- Frontend ↔ Commerce Service
	- Frontend ↔ Lottery Service

3. Event-driven architecture
	- `OrderCreated`
	- `CartUpdated`
	- `LotteryClosed`
	- `GiftAdded`
	- `DonorRegistered`

4. Saga / Orchestration
	- תהליך רכישה של סל `CardCart`/`PackageCart`
	- `PaymentReservation` → `InventoryLock` → `CardAssignment` → `OrderConfirmation`

5. Outbox pattern
	- שמירת event ל־DB של Service לפני שליחתו ל־message broker
	- מבטיח idempotency ו־transactional consistency

6. Anti-corruption layer
	- בזמן מעבר ממונולית ל־microservices, לשמור על Adapter/Facade עבור ממשקים קיימים

7. Data ownership ו־Database-per-service
	- כל שירות הוא בעל הנתונים שלו
	- אין גישה ישירה ל־DB של שירות אחר

8. Resilience patterns
	- Circuit Breaker
	- Retry עם exponential backoff
	- Bulkhead isolation

---

## הערות ארכיטקטוניות

- ההפרדה בין `Catalog Service` ל־`Commerce Service` מאפשרת סקיילינג לפי נפח קריאות Catalog מול פעולות סלים.
- `Lottery Service` צריך להיות בעל לוגיקה עצמאית כדי לנהל סטטוס של הגרלות ללא תלות במצב הקטלוג.
- `File Service` משתמש ב־blob storage כדי להפחית עומס על ה־DB ולהאיץ טעינת מדיה.
- אם רוצים ליישם `Auth` מתקדמת, אפשר להפריד אותה ל־Identity Service נפרד עם OAuth2 / JWT.

