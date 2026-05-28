CREATE TABLE Customers (
    ID int PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    inn VARCHAR(12),
    address VARCHAR(255),
    phone VARCHAR(20),
    is_salesman BIT,
    is_buyer BIT
);

CREATE TABLE Products (
    ID INT PRIMARY KEY,  -- В PostgreSQL: SERIAL PRIMARY KEY
    product_name VARCHAR(255) NOT NULL,
    code VARCHAR(50),
    unit VARCHAR(20)
);

CREATE TABLE Materials (
    ID INT PRIMARY KEY,  -- В PostgreSQL: SERIAL PRIMARY KEY
    material_name VARCHAR(255) NOT NULL,
    code VARCHAR(50),
    unit VARCHAR(20),
    price DECIMAL(10,2)
);

CREATE TABLE Recipes (
    ID INT PRIMARY KEY,  -- В PostgreSQL: SERIAL PRIMARY KEY
    product_id INT NOT NULL,
    material_id INT NOT NULL,
    quantity_per_unit DECIMAL(10,3) NOT NULL,
    FOREIGN KEY (product_id) REFERENCES Products(ID),
    FOREIGN KEY (material_id) REFERENCES Materials(ID)
);

CREATE TABLE Orders (
    ID INT PRIMARY KEY,
    customer_id VARCHAR(9) NOT NULL,
    order_date DATE NOT NULL,
    total_amount DECIMAL(10,2),
    FOREIGN KEY (ID) REFERENCES Customers(ID)
);

CREATE TABLE OrderDetails (
    ID INT PRIMARY KEY,  -- В PostgreSQL: SERIAL PRIMARY KEY
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity DECIMAL(10,2) NOT NULL,
    unit_price DECIMAL(10,2),
    FOREIGN KEY (order_id) REFERENCES Orders(ID),
    FOREIGN KEY (product_id) REFERENCES Products(ID)
);

