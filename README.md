# ThAmCo Web Application

Welcome to ThAmCo Web App! This web application provides a user-friendly interface for accessing and managing product details. It serves as a front-end to the ProductsAPI, enabling both public and 
staff-specific functionalities securely.

## Features

- **User Authentication**: Secure login system for both public users and staff.
- **Product Browsing**: Users can browse products and search by name or description.
- **Product Management**: Staff with privileged roles can create, update, and delete products.
- **Responsive Design**: Compatible with various devices and screen sizes.
- **Security**: Integrated security features to safeguard data and user interactions.

## Requirements

- ASP.NET MVC
- .NET Core 8.0 or later
- ProductsAPI (back-end setup)
- SQL Server (for user and product data)
- Modern web browser

## Getting Started

Here are the steps to set up the web application locally:

1. **Clone the repository**:
   ```bash
   git clone [repository-url]
2. **Restore dependancies**:
   ```bash
   dotnet restore
3. **Build the project**:
   ```bash
   dotnet build
4. **Run the application**:
   ```bash
   dotnet run

## Usage
After launching the web app, you can:

1. Log in as a public user or staff member to access different functionalities.
2. View Products: Browse all products available in the database.
3. Search Products: Use the search bar to find products by name or description.
4. Manage Products (Staff only): Add new products, edit existing product details, or remove products from the inventory.

## Contributing
Interested in contributing? Here's how you can help:

1. Fork or branch the repository.
2. Make changes and ensure tests pass in ProductsAPI.Test.
3. Submit a pull request detailing your changes.
