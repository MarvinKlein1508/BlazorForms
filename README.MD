# BlazorForms
Create dynamic PDF forms completely in the web.

![Dashboard](images/dashboard.png)
![Form builder](images/form_builder.png)
![Element editor](images/element_editor.png)
![Calc editor](images/calc_editor.png)
![New form entry](images/new_form_entry.png)
![New form entry tables](images/new_form_entry_table.png)

## Requirements
* MariaDB Database
* ASP.NET Core 8 Hosting Bundle x64

## Features
* Login via LDAP or local account
* Create unlimited amount of forms
* Submitted forms can be edited afterwards
* Supports both German & English
* Forms can be limited to fill out by certain users or logged in users
* Define managers for forms who can see all submitted entries
* E-Mail notifications for form managers
* All entries are downloadable as PDF
* Upload custom logo and images for each form
* Rich form builder with 12 custom elements
* Form builder is fully usable on both mobile and desktop
* Define rules to display certain elements, rows and columns
* Integrated file upload component (Supports 1162 file types)
* Search for form entries
* Overview of your own submitted forms
* Changes to forms doesn't affect past form entries
* Many validation options (e.g. REGEX)
* Calc values of elements based on other element values
* Support for SUM in tables
* Support for GIFs in the dashboard

## Installation
First provide your connection string, email and LDAP configuration in `appsettings.json`. 

Next run the installer 
```
BlazorForms.exe -setup
```

Choose option 2 to create the initial database. If you want to use a local administrator choose option 1 next.

## Hosting
Hosting this app is quite simple. It supports all hosting possibilities from ASP.NET Core. For more information take a look on the [official docs from Microsoft](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-7.0).

## Recommendations
This app is toring uploaded files within the database. The default limit from MariaDB is 1MB per request. We highly recommend you increase this setting on your server to something which fits your use case. This limit can be increased by changing the value of `max_allowed_packet` from your MariaDb server.

This can usually be done within the `my.ini` file.


## NOT READY YET
This Software is still in development and should not be used right now since it's missing some major core functionalities.