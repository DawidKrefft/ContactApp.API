# Contact List Web Application TASK

## Functionalities

1. **Login:**
   - Functionality in point 2 is available to unauthenticated users; the rest requires authentication.

2. **Browsing the Contact List:**
   - The list should contain basic data. When selecting a specific contact, their details are displayed.

3. **Contact Details:**
   - Authenticated users can edit, delete existing entries, and add new ones. Each individual contact should have at least:
     - First name,
     - Last name,
     - Email (unique),
     - Password (must meet basic password complexity standards),
     - Category (work, private, other),
     - If "work" is selected, the option to choose a subcategory from a dictionary (e.g., boss, client, etc.) should be available. For "other," the ability to enter any subcategory,
     - Phone number,
     - Date of birth.

## Technical Requirements

- The application should be written in C# and use any database.
- Backend architecture - REST API.
- Frontend architecture - Single Page Application.
- All dictionary data (categories, subcategories) should be stored in the database.
- It is recommended to use open-source libraries.
- Pay attention to the security of the application.
- The source code should contain comments.
- The visual design of the application is not significant.
