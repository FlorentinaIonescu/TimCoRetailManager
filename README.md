# TimCo Retail Manager
"A retail management system built by TimCo Enterprise Solutions"

Following Tim Corey's tutorial series, I've created this solution in .NET Framework 4.7.2, before upgrading the front-end class library (TRMDesktopUI.Library) and the back-end class library (TRMDataManager.Library) to .NET Standard 2.0, while the UI and the API are now .NET Core 3. There is also a SQL Server database. Caliburn Micro dependency injection is involved. Visual Studio 2019, Swagger and, separatedly from the app, Postman have been used.

# TimCoRetailManager Phase 1 Complete + Core Upgrade

State of completeness: phase 1 complete, core upgrade and some bug fixes from phase 2, as well as a couple of original features (loading indicator on the login screen, button on the user view for returning to sales view). Potentially needs extra bug fixes.

# What is TimCo Retail Manager

The tutorial is designed to roughly simulate the development stage of a real-world application.
TimCo Retail Manager is a basic retail management system where the user can select products and item quantity, add or remove them from a shopping cart, and then proceed to checkout.

Building the solution initializes a web API and a WPF-based application shell view with a menu (both are startup projects). Inside the shell view, what's initially displayed is the login view. Upon successful login, the user gains access to the sales view.
From the toolbar menu, the user may select User Management and, depending on their role, may view a list of all the user accounts and add/remove their roles.
There are three roles, in the role of descending number of rights: "Admin", "Manager", and "Cashier".
As an useful addition to the user view, there's an ugly button that allows for returning to the sales view without having to log out and log in again.
The worth of transactions is calculated with a tax rate, from which certain products may be exempt.

# Solution Structure (5 projects)

In Visual Studio, there are four visible folders representing five projects: "API" (includes class library), "Database", "Solution Items" (which only includes a roadmap text concerning phase 2), and "WPF" (includes class library). "TRMApi" and "TRMDesktopUI" are the startup projects in Visual Studio.

The API allows exchange of information between the front-end and the database without unnecessarily exposing the latter; it is also used for registering user accounts.
Our web API consists of Program and Startup, a .json file containing app settings (connection strings, security key, tax rate), and most importantly, a number of controllers and models, as well as views for visualizing the API in a web browser. There is also a folder, ”Data”, which contains the class ApplicationDbContext, used within the controllers, as well as a subfolder, ”Migrations”, as a result of migrating from the former ASP.NET database based on Entity Framework.
Packages include Microsoft.AspNetCore.Authentication.JwtBearer, Swashbuckle.AspNetCore.Swagger, System.IdentityModel.Tokens.Jwt, etc.
The web API runs on Kestrel.

The six controllers are HomeController (self-generated code), InventoryController, ProductController, SaleController, TokenController, and UserController.
- InventoryController has two methods, one for retrieving the entire inventory of the retail system, and the other for saving an inventory record.
- ProductController has a method for retrieving the list of available products from the inventory upon loading the sales view.
- SaleController features a method for retrieving tax rate (for determining it, see code behind in the class library) and one for saving sale data, as well as a method for allow administrators and managers to retrieve sales reports (function only available in Swagger currently). 
- TokenController has the back-end code for generating a token based on Jwt authentication, as well as an async task for validating user name and password values entered in the API.
- UserController features four functions: GetById, which matches user id when logging in; GetAllUsers, which retrieves a list of all user accounts to display in the user administration view; GetAllRoles, triggered by selecting a user account, retrieves the list of roles corresponding to the selected account; AddARole and RemoveARole, which are self-explanatory (user roles).
The models ApplicationUserModel, ErrorViewModel, and UserRolePairModel are all used in the user administration view.

The code behind for the API is stored in the class library: data access files, an additional internal data access file pertaining to the SQL connection (marked as "internal" despite class being public to work with dependency injection), and the models.
- InventoryData contains two methods, one for retrieving the inventory from the SQL database, while the other saves the inventory record.
- SaleData, from which interface ISaleData has been extracted, features the code for determining tax rate (as decimal), as well as for creating and saving a sale model; GetSaleReport implements the store procedure needed to create a sales report.
- ProductData retrieves the list of all products as well as matching the id of a given product.
- In UserData, GetUserById looks up in the database the user model that mathches the given id

SqlDataAccess deals with everything pertaining to data exchange with the SQL server, from determining the right connection string to committing transaction, or rolling it back in case connection suddenly fails.
- LoadData method connects to the database, does a query, determines the type of model each row should be, passes stored procedure name and parameteres as well as the connection string name, determines stored procedure and returns a set of rows.
- With Execute instead of query, and without passing parameter U, SaveData is otherwise similar.
- StartTransaction, LoadDataInTransaction and SaveDataInTransaction pertain to sales.
The structure of a transaction is: start transaction, load its data, save data, close/stop connection, dispose.

Models included: InventoryModel, ProductModel, SaleDBModel, SaleDetailDBModel, SaleDetailModel, SaleModel, SaleReportModel, UserModel.

The UI is a WPF application implementing MVVM.
Models: CartItemDisplayModel (details for each product in the shopping cart), ProductDisplayModel (details for each product from the inventory on sale).
Views: ShellView (with top menu), LoginView (logging into account), SalesView (shopping), StatusInfoView (for system message/error), UserDisplayView (user administration).
View Models: ShellViewModel (initializes most of the other views based on login credentials, allows for exiting application), LoginViewModel (for logging in, see below; includes error display, as well as a "loading" text to confirm button was clicked and something's happening!), SalesViewModel (for viewing products from inventory, adding to/removing from cart, checking out the contents of the cart; subtotal, tax, and total calculations; booleans determine if/when buttons can be used; ResetSalesViewModel is used to refresh lists after checkout), StatusInfoViewModel, UserDisplayViewModel (for loading user accounts and their roles, selecting acounts, adding and removing roles from the accounts).
Other folders: EventModels (empty event classes: LogOnEvent), Helpers (PasswordBoxHelper, for registering/changing password).
* Additions to Tim's code: "loading" status in the login view, "Return to Shopping" button in the user administration view, for returning to sales view. *

Bootstrapper determines display root view on startup (ShellViewModel), configures AutoMapper (between ProductModel and ProductDisplayModel, CartItemModel and CartItemDisplayModel), container instances, singletons, and sets up reflection.

The UI's class library features the code behind for UI models, (I)ConfigHelper, and the API endpoints (see Swagger) and their interfaces.
- APIHelper initializes client, deals with authentication and getting login user info.
- ProductEndpoint retrieves a list of all products.
- SaleEndpoint features async task for posting sale (note placeholder comment for logging successful call).
- UserEndpoint features tasks involved in user administration: get all users, get all roles, add user to role, remove user from role.

There are two databases. "TRMData" stores the data necessary for the retail system itself. "ApiAuthDb" (which, upon upgrading from .NET Framework, replaces "EFData", EF standing for Entity Framework) handles the information associated with the user accounts.
- ApiAuthDb stores the following data: migration history, role claims, roles, user claims, user logins, user roles, users, user tokens.
- TRMData features the tables: inventory, products, sales, sale details, users. (Minor bug: "SaleTable" and "UserTable" persisting empty duplicates, result of improper naming when having created the stored procedures for the first time.) Stored procedures: Inventory_GetAll and _Insert, Product_GetAll and _Insert, Sale_Insert, _Lookup and _SaleReport, SaleDetail_Insert, and UserLookup.
Note: when setting up code and registering in the API authentication database, it is needed to manually copy the user data from the table of that database into the other database's user table.

# How Login Works

- Caliburn Micro's SimpleContainer and ConventionManager are priorly setup in Bootstrapper.
- User enters user id (email) and password into their corresponding text boxes (which will be stored in the variables).
- In the XAML of the WPF view there is a specification that its DataContext is LoginViewModel.
- Login button raises event, Caliburn Micro matches command to LogIn() task, which will then make the loading indicator visible for the user and store potential error message.
- In order to assign variable result, we go into Authenticate task from APIHelper.
- Key value pairs are encoded for grant type, username and password.
- A HTTP request is made to the web API for posting token.
- Token controller finds and checks user name and password, then a token is generated corresponding to them.
- If token request was successful, another response is queued to read authenticated user model.
- To capture information about the user, the bearer token will be used to authorize logged in user info (HTTP request to the API endpoint).
- To find the value of the user, a connection is established with the SQL server.
- In UserData, an anonymous object is initialized for the Id before dynamically loading data obtained through SQL query.
- If request was successful, LoggedInUserModel is being read and then assigned stored information values returned by the database.
- Login process is finished after publishing it on UI thread.
- Sales view data is loaded upon login.
If an HTTP request returns error, or if a task is cancelled during debugging, corresponding message is shown in red above the text boxes on the login view, and the loading indicator below the button disappears.

# Known bugs

- Log In menu button is not properly hidden in toolbar while in sales view.
- Pressing Log Out menu button returns the user to the sales view, prompting error that unauthorized user is not allowed to view the sales view, blanking out the shell view container.
- When selected item quantity is left default, user must type in ”1” again, or a different desired number, for the Add to Cart button to work.
- It is possible for an Admin user to remove their own Admin role and cut off access to user administration.
- No confirmation for finished transaction other than resetting sales view.