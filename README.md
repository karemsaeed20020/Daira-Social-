<!-- Project Badges -->
<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0" />
  <img src="https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C# 12" />
  <img src="https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Entity%20Framework-8.0-512BD4?style=for-the-badge&logo=nuget&logoColor=white" alt="Entity Framework Core" />
  <img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT Auth" />
  <img src="https://img.shields.io/badge/SignalR-Real--Time-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="SignalR" />
  <img src="https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger" />
</p>

<!-- Project Title -->
<h1 align="center">🌐 Daira</h1>

<p align="center">
  <strong>A Modern Social Media Platform Backend Built with Clean Architecture</strong>
</p>

<p align="center">
  Daira is a production-ready, enterprise-grade social media platform backend API built with ASP.NET Core 9.0, following Clean Architecture principles and SOLID design patterns. The platform provides comprehensive social networking features including real-time messaging, post management, user interactions, and notification systems, all designed for scalability, maintainability, and extensibility.
</p>

---

## 📑 Table of Contents

- [Overview](#-overview)
- [Key Highlights](#-key-highlights)
- [Features](#-features)
  - [Authentication & Authorization](#authentication--authorization)
  - [User Management](#user-management)
  - [Posts & Feed](#posts--feed)
  - [Social Interactions](#social-interactions)
  - [Real-Time Messaging](#real-time-messaging)
  - [Notifications](#notifications)
  - [Administration](#administration)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Technologies Used](#-technologies-used)
- [Prerequisites](#-prerequisites)
- [Installation & Setup](#-installation--setup)
- [Configuration](#-configuration)
- [API Endpoints](#-api-endpoints)
- [Real-Time Hub Methods](#-real-time-hub-methods)
- [Usage Examples](#-usage-examples)
- [Contributing](#-contributing)

---

## 🔭 Overview

### What is Daira?

Daira (Arabic for "Circle") is a comprehensive social media platform backend designed to power modern social networking applications. It provides all the essential building blocks for creating engaging social experiences, from user authentication to real-time messaging.
### Business Domain

The platform operates in the **social networking domain**, enabling users to:
- Connect with friends through follow and friendship systems
- Share content through posts with likes and comments
- Communicate in real-time through direct and group messaging
- Stay informed through an intelligent notification system

### Target Users

- **Frontend Developers**: Building web or mobile social media applications
- **Startups**: Looking for a robust, scalable backend foundation
- **Enterprises**: Needing internal social networking solutions
- **Educational Projects**: Learning Clean Architecture with real-world examples

### Problem Solved

Daira addresses the complexity of building social media backends by providing:
- **Pre-built social features**: No need to reinvent common patterns
- **Scalable architecture**: Designed for growth from day one
- **Real-time capabilities**: Native WebSocket support for instant updates
- **Security-first approach**: JWT authentication with refresh tokens
- **Clean codebase**: Easy to extend and maintain

---

## ✨ Key Highlights

- 🔐 **Enterprise-Grade Security**: JWT authentication with access/refresh token rotation, email confirmation, and password reset flows
- 🏗️ **Clean Architecture**: Strict separation of concerns with Domain, Application, Infrastructure, and Presentation layers
- ⚡ **Real-Time Communication**: SignalR-powered messaging with typing indicators, read receipts, and presence detection
- 📊 **Specification Pattern**: Flexible and reusable query specifications for complex data filtering
- 🔄 **Unit of Work Pattern**: Transactional consistency across repository operations
- 📧 **Email Integration**: MailKit-based email service for confirmations and notifications
- ✅ **Input Validation**: FluentValidation for robust request validation
- 🗺️ **Object Mapping**: AutoMapper for clean DTO transformations
- 📝 **API Documentation**: Swagger/OpenAPI with JWT authorization support
- 🌐 **CORS Support**: Configurable cross-origin resource sharing

---

## 🚀 Features

### Authentication & Authorization

| Feature | Description |
|---------|-------------|
| User Registration | Complete registration with email confirmation requirement |
| Login/Logout | Secure authentication with JWT token generation |
| Refresh Tokens | Silent token renewal with 7-day refresh token validity |
| Email Confirmation | Token-based email verification flow |
| Password Reset | Secure forgot/reset password with email tokens |
| Resend Confirmation | Ability to resend confirmation emails |

### User Management

| Feature | Description |
|---------|-------------|
| Account Management | User profile and account settings |
| Role-Based Access | Flexible role assignment and management |
| User Roles Query | Retrieve roles for specific users |
| Role CRUD | Create, read, update, and delete roles |
