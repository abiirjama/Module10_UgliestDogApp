using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

// PageModel for handling the Ugliest Dogs page
public class UgliestDogsModel : PageModel
{
    // List populated from the database to show dog names in the dropdown menu
    public List<SelectListItem> DogList { get; set; }

    // Stores the dog that the user selects from the dropdown
    public Dog SelectedDog { get; set; }

    // Runs automatically on first page load (HTTP GET request)
    public void OnGet()
    {
        LoadDogList(); // Load dropdown list of dogs from the database
    }

    // Runs when user submits the selection (HTTP POST request)
    public void OnPost(string selectedDog)
    {
        LoadDogList(); // Reload dropdown list after postback so the dropdown keeps its data

        // Check if user selected a dog from the dropdown
        if (!string.IsNullOrEmpty(selectedDog))
        {
            SelectedDog = GetDogById(int.Parse(selectedDog)); // Get full dog details from the database
        }
    }

    // Retrieves all dogs from the database and fills dropdown list items
    private void LoadDogList()
    {
        DogList = new List<SelectListItem>(); // Creates an empty list

        using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
        {
            connection.Open(); // Open database connection
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Dogs"; // Select only ID and Name for dropdown

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DogList.Add(new SelectListItem
                    {
                        Value = reader.GetInt32(0).ToString(), // First column = Id
                        Text = reader.GetString(1)            // Second column = Name
                    });
                }
            }
        }
    }

    // Retrieves all details of a single dog based on the selected ID
    private Dog GetDogById(int id)
    {
        using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
        {
            connection.Open(); // Open database connection
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Dogs WHERE Id = @Id"; // Get full row where Id matches
            command.Parameters.AddWithValue("@Id", id); // Prevents SQL injection

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Dog
                    {
                        Id = reader.GetInt32(0),      // Dog ID
                        Name = reader.GetString(1),   // Dog name
                        Breed = reader.GetString(2),  // Dog breed
                        Year = reader.GetInt32(3),    // Dog year
                        ImageFileName = reader.GetString(4) // Image file name stored in DB
                    };
                }
            }
        }
        return null; // Return null if dog not found
    }
}

// Represents one dog record returned from the database
public class Dog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Breed { get; set; }
    public int Year { get; set; }
    public string ImageFileName { get; set; }
}
