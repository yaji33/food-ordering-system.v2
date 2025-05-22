ALTER USER 'root'@'localhost' IDENTIFIED BY 'yajidev';
CREATE DATABASE demodb;
GRANT ALL PRIVILEGES ON demodb.* TO 'root'@'localhost';
FLUSH PRIVILEGES;
