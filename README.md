# PROG6212_POEPART2_ST10456109_MusaJanda

## Overview 🚀

CMCS is a web-based application designed to streamline the process of submitting and approving monthly claims for contract lecturers. The system replaces a manual, paper-based workflow with a digital solution, improving efficiency and providing a clear audit trail.

This project was developed using **ASP.NET Core MVC** with **Entity Framework Core** and **ASP.NET Identity** for user management and role-based authorization.

---

## Features ✨

### Part 1 & 2 Combined Features

* **Role-Based Dashboards:** Users see a dashboard tailored to their role (Lecturer, Programme Coordinator, or Academic Manager).
* **Secure Authentication:** Secure user authentication and management are handled by ASP.NET Identity.
* **Claim Submission:** Lecturers can digitally submit their monthly claims, including hours worked, and attach supporting documents (e.g., claim forms).
* **Document Management:** The system supports uploading and storing supporting documents related to claims.
* **Multi-Stage Approval Workflow:** Claims follow a defined workflow:
    1.  **Lecturer** submits a claim.
    2.  **Programme Coordinator** reviews and approves the claim.
    3.  **Academic Manager** gives the final approval.
   

### Part 2 Enhanced Functionalities 🆕

* **Advanced Claim Management:**
  - **Create Claims:** Lecturers can create claims
  - **Edit Claims:** Lecturers can modify their submitted claims before they are approved
  - **Department Assignment:** Claims are now associated with specific departments (required field)
  - **Automatic Calculations:** Real-time calculation of total amount (Hours × Rate)
  - **Enhanced Validation:** Comprehensive form validation with clear error messages

* **Financial Features:**
  - **Hourly Rate Management:** Configurable hourly rates per lecturer/contract
  - **Total Amount Calculation:** Automatic computation of claim totals
  - **Currency Formatting:** Proper display of monetary values (e.g., R 400.00)

* **Improved User Experience:**
  - **Real-time Form Validation:** Immediate feedback on form errors
  - **Dynamic Calculations:** Automatic total amount updates as users input hours and rates
  - **Enhanced Error Handling:** User-friendly error messages with field-specific guidance
  - **Document Protection:** Supporting documents cannot be modified after submission to maintain audit integrity

* **Data Integrity & Business Rules:**
  - **Department Validation:** Department selection is mandatory for claim submission
  - **Claim Date Tracking:** Proper date validation and formatting
  - **Hours Worked Validation:** Ensures positive, reasonable hour entries
  - **Role-Based Access Control:** Strict permission management throughout the workflow

* **Administrative Controls:**
  - **User Role Management:** Admin can assign and modify user roles
  - **Department Management:** System supports multiple departmental structures
  - **Audit Trail Maintenance:** Complete history of claim modifications and approvals

---

## Getting Started ⚙️

### Prerequisites

* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or a compatible database)
* [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)

## How to use the CMCS System

### For Lecturers 👨‍🏫
1. **Register/Login:** Create an account with Lecturer role and log in
2. **Submit Claim:** 
   - Click "Claim" button to access claim form
   - Select your **Department** (required field)
   - Enter Claim Date, Hours Worked, and Hourly Rate
   - System automatically calculates **Total Amount**
   - Add description of work performed
   - Upload supporting documents
3. **Edit Claims:** Modify your submitted claims before they are approved
4. **Track Status:** Monitor claim progress through approval workflow

### For Programme Coordinators 👨‍💼
1. **Login:** Access system with Coordinator credentials
2. **Review Claims:** View claims submitted by lecturers in your department
3. **Approve/Reject:** Make initial approval decisions on claims
4. **Provide Feedback:** Add notes when rejecting claims to guide lecturers

### For Academic Managers 🎓
1. **Login:** Access system with Manager credentials
2. **Final Approval:** Review Coordinator-approved claims
3. **Final Decision:** Provide final approval or rejection
4. **Oversight:** Monitor overall claim workflow and departmental activities

### For Administrators ⚙️
1. **User Management:** Assign and modify user roles
2. **System Configuration:** Manage departments and system settings
3. **Troubleshooting:** Assist users with account and access issues

---

## Key Workflows 🔄

### Claim Submission & Editing

** Lecturer Login → Dashboard → Create Claim → Fill Form (Department*, Date, Hours, Rate)
→ Automatic Calculation → Add Description → Upload Documents → Submit


### Approval Workflow

** Lecturer Submits → Programme Coordinator Reviews → Approve/Reject
→ Academic Manager Final Review → Final Approval/Rejection


## Database Management

The application uses Entity Framework Core to manage the database schema and data, with a clear separation of models for:
- **Claims** (with enhanced fields: Department, Hours, Rate, Total Amount)
- **Documents** (secure storage with modification restrictions)
- **Lecturers** (with departmental associations)
- **User Roles & Profiles** (ASP.NET Identity integration)
- **Departments** (departmental structure management)

**Version:** 2.0  
**Last Updated:** 20 October 2025  
**Developer:** Musa Janda (ST10456109)
