# ThAmCo Web Application

This is a cloud-hosted, role-secured web application designed to allow both customers and staff to interact with product data in a streamlined and secure environment. Built using ASP.NET Core MVC, the application serves as the front-end to the underlying **ProductsAPI**, which manages the core product-related operations.

The front-end communicates asynchronously with the ProductsAPI via RESTful HTTP calls. Staff members with appropriate roles can perform full product management (create, update, delete), while general users can browse and search the product catalogue. All user interactions are protected with robust **OIDC and OAuth2-based authentication and authorisation**.

To promote scalability and clean architecture, the system implements the **Remote Façade** and **Repository** design patterns:
- The *Remote Façade* pattern exposes a simplified interface to the ProductsAPI, hiding the complexity of underlying HTTP logic.
- The *Repository* pattern decouples the domain logic from data access and API integration, making the application easier to test and maintain.

In addition, **network resilience** is achieved through:
- **Retry Policies**: Automatically retry transient failures such as brief API outages or timeouts.
- **Circuit Breaker**: Prevents cascading failures by temporarily halting requests when repeated failures are detected, allowing external services to recover.

These mechanisms are implemented using middleware and policy-based HTTP clients, ensuring fault tolerance and a seamless user experience.


## Images

### Manage Products
<img src="https://github.com/user-attachments/assets/487495f8-6ce6-47f5-9e99-4413fe0474da" alt="Manage products" width="50%"/>

### Role-Based Access Control
<img src="https://github.com/user-attachments/assets/fd2feda9-8c1e-46ee-ba8f-63ea8b5bb584" alt="Role-based access control" width="50%"/>

## Features

- **User Authentication**: Secure login system for both public users and staff.
- **Product Browsing**: Users can browse products and search by name or description.
- **Product Management**: Staff with privileged roles can create, update, and delete products.
- **Responsive Design**: Compatible with various devices and screen sizes.
- **Security**: Integrated security features to safeguard data and user interactions.

## Live Project

Access the deployed version here:  
[**ThAmCo Web App**](https://thamco-webapplication-aegxdgbkdycgaqcq.uksouth-01.azurewebsites.net/)

## Requirements
- .NET Core 8.0 or later  
- ProductsAPI (back-end setup)  
- SQL Server (for user and product data)  
- Modern web browser  

## Getting Started

To set up the web application locally:

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
