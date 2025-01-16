# RiffFinder

RiffFinder is a music collaboration platform designed to help musicians connect, create bands, and manage postings for finding new members.

---

## **Getting Started**

### **Prerequisites**

Ensure you have the following installed:

1. [.NET 7.0 SDK](https://dotnet.microsoft.com/download)
2. [Node.js (LTS)](https://nodejs.org/)
3. [SQL Server](https://www.microsoft.com/en-us/sql-server)

---

### **Backend Setup**

1. Go to rest-api folder:
   ```bash
   cd rifffinder/rest-api
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Configure environment variables:
   - Create a `appsettings.json` file in the `rest-api` directory based on the `appsettings.example.json`.

4. Apply migrations and seed the database:
   ```bash
   dotnet ef database update
   dotnet run --seed
   ```

5. Run the backend:
   ```bash
   dotnet run
   ```

---

### **Frontend Setup**

1. Navigate to the `client` folder:
   ```bash
   cd ../client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the frontend:
   ```bash
   npm start
   ```

4. Access the app:
   - Visit `http://localhost:3000` in your browser.

---

## **Business Rules and Logic**

### **1. Bands**
A musician can only create a band if they are not already in one. When a band is created, all the musician's pending requests to other bands are automatically deleted. Musicians can leave their band, and the band is deleted if no members remain.

### **2. Postings**
Only musicians belonging to a band can create postings. Postings are automatically marked as closed when a request is accepted. Only open postings are displayed to musicians. 

### **3. Requests**
Musicians cannot send requests to join their own band's posting or if they already belong to another band. Only a band member can accept or deny requests associated with their band's postings.

### **4. Profiles**
Each musician has a profile that displays their name, instrument, and band (if applicable). Bands display details like name, genre, bio, and members.


## **API Documentation**

### **Swagger**
- Local: `http://localhost:7290`

