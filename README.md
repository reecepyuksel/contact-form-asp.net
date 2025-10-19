# ASP.NET Contact Form - Dockerized Version

Bu proje, kullanıcıların web üzerinden iletişim formu aracılığıyla mesaj göndermesini sağlayan bir **ASP.NET Core MVC** uygulamasıdır.  
Uygulama, **Microsoft SQL Server (Docker)** üzerinde çalışır ve container ortamında otomatik olarak başlatılabilir.

---

## 1. Proje Tanıtımı

Bu uygulama, gelen mesajları veritabanında saklayan basit ama genişletilebilir bir iletişim formu sistemidir.  
Amaç, **Docker kullanarak** kolayca kurulabilir ve her ortamda aynı şekilde çalışan bir ASP.NET uygulaması oluşturmak.

### Özellikler
- Departman seçimi (örnek: Muhasebe, Teknik Destek, İnsan Kaynakları)
- SQL Server üzerinde otomatik migration ve seed verileri
- RESTful API endpoint'leri (`/api/contactmessages`, `/api/departments`)
- Docker Compose ile tek komutla ayağa kaldırılabilen yapı

---

## 2. Kullanılan Teknolojiler

![C#](https://img.shields.io/badge/C%23-12-blue?logo=csharp)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![EntityFramework](https://img.shields.io/badge/EntityFrameworkCore-8.0-512BD4)
![SQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-2022-red?logo=microsoftsqlserver)
![Docker](https://img.shields.io/badge/Docker-Containerization-2496ED?logo=docker)
![HTML](https://img.shields.io/badge/HTML-5-orange?logo=html5)
![CSS](https://img.shields.io/badge/CSS-3-blue?logo=css3)
![Git](https://img.shields.io/badge/Git-Version%20Control-F05032?logo=git)

---

## 3. Diyagramlar

### Kullanıcı Arayüzü
![User Interface](images/userinterface.jpg)

### Veritabanı Diyagramı
![Database Diagram](images/databasediagram.png)

### Departman İlişkisi
![Department Diagram](images/diagram_dep.png)

### Dataset Akışı
![Dataset Diagram](images/diagram_dataset.png)

---

## 4. Kurulum ve Çalıştırma

### Gereksinimler
- Docker ve Docker Compose kurulu olmalıdır.  
- İnternet bağlantısı gereklidir (SQL Server imajı indirilecektir).  

---

### Adım 1: Depoyu Klonlayın
```bash
git clone https://github.com/reecepyuksel/contact-form-asp.net
cd contact-form-asp.net
```

---

### Adım 2: `.env` Dosyasını Oluşturun
Proje dizininde `.env` adında bir dosya oluşturun ve aşağıdaki bilgileri ekleyin:

```env
DB_SERVER=mssql,1433
DB_NAME=aspnet_contact_form
DB_USER=SA
DB_PASS=1234
```

> Parolayı güvenli bir şekilde değiştirmeniz önerilir.

---

### Adım 3: Docker Compose ile Başlatın
Aşağıdaki komut, hem SQL Server’ı hem de ASP.NET uygulamasını başlatır:

```bash
docker compose up -d --build
```

Başlatma tamamlandığında uygulamaya şu adresten erişebilirsiniz:  
[http://localhost:8080](http://localhost:8080)

---

### Adım 4: Servis Durumunu Kontrol Etme
Servislerin durumunu görmek için:
```bash
docker ps
```

Log’ları görmek için:
```bash
docker logs aspnet-contact-form
```

Durdurmak için:
```bash
docker compose down
```

---


## 5. Proje Yapısı

```
project-root/
│
├── aspnet-contact-form/
│   ├── aspnet-contact-form.sln
│   ├── Dockerfile
│   └── aspnet-contact-form/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       ├── Migrations/
│       ├── Views/
│       ├── Program.cs
│       ├── appsettings.json
│       └── .env
│
├── docker-compose.yml
├── images/
│   ├── databasediagram.png
│   ├── diagram_dataset.png
│   ├── diagram_dep.png
│   └── userinterface.jpg
└── README.md
```
