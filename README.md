# ECommerceWebApp

Welcome to ECommerceWebApp, an ASP.NET Core MVC application for managing an e-commerce platform. This repository contains the source code and configuration files for the application.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Usage](#usage)
- [Setup](#Setup)
- [Technologies Used](#Technologies-Used)

## Overview

ECommerceWebApp is a web application built using ASP.NET Core MVC. It allows users to register, log in, manage products, browse categories, place orders, and perform various actions on the platform. Admin users have additional access to manage users and perform administrative tasks.

## Features

- User registration and authentication
- User roles (admin and regular users)
- Product management
- Category browsing
- Order placement
- User account details and editing
- Admin panel for user management
- Responsive design for different screen sizes

## Usage

- Register as a new user or log in using your credentials.
- Browse products and categories, and add items to your cart.
- Place orders for selected products.
- Admin users can manage users and perform administrative tasks from the admin panel.
- Update your account details and view your order history.

## Setup

1. Clone this repository to your local machine.
2. Install the required dependencies using NuGet Package Manager.
3. Configure the database connection in the `appsettings.json` file by updating the `ConnectionStrings` section.
4. Run database migrations to create the database schema: `dotnet ef database update`.
5. Build and run the application: `dotnet run`.

## Technologies Used

- ASP.NET Core MVC: Handling web requests and rendering views.
- Entity Framework Core: Managing the database schema and data access.
- Razor Pages: Creating dynamic web pages with server-side rendering.
- Bootstrap: Styling the user interface for responsiveness.
- Session Management: Managing user sessions and cart contents.
- File Uploads: Uploading and managing product images.
- Role-Based Authorization: Implementing access control for different user roles.

Happy coding!
