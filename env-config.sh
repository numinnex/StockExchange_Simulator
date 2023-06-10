#!/bin/bash
sed -i "s|Server=localhost,1433;Database=CleanArchitectureDb;User Id=sa;Password=pa55w0rd!D;Trust Server Certificate=True|$DefaultConnectionString|g" /app/appsettings.json

