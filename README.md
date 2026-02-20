# Hospital Management System

## Context

* **Origin** Developed as part of the third C# course at Malmö University.
* **Objective** This web application represents a Hospital Management System with patient and doctor auth access.
* **Status** 🟢 Complete/Functional

---

## Systems Architechture

* **Logic** .NET Web App + MVC architechture with ViewModels
* **Tech Stack** C# + .NET Web App + EF Core + SQLServer + Cookie based auth

---

## Functionality

* Role based access
* Users not logged in can:
  - Navigate the about-pages and register as patients
* Patients logged in can:
  - Edit their personal information
  - Request medical appointments
  - Manage existing appointments
  - View medical history
* Doctor users can:
  - Confirm appointments
  - Edit patients' medical history
  - View upcoming and finalized appointments
  - View a dashboard of statistics (placeholders)

---

## Setup & Usage

1. Clone the repository
2. Open in IDE of your choice
3. Become a patient or doctor!

---

## Learning Outcomes

* Async programming
* HTML with bootstrap shorthand
* MVC + View Models setup
* Cookie based auth
* Layered architechtural approach within MVC (ex: DB -> Repository -> Service -> Controller -> Service)
