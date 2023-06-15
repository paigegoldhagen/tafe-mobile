using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace MobileApp;

public partial class AddEmployee : ContentPage
{
    public List<EmployeeInfo> newEmployee = new();
    public List<EmployeeInfo> employeeList = new();

    public AddEmployee()
	{
		InitializeComponent();
	}

    // Get user inputs and add to local employees.csv
    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        FileImageSource getNewImage = (FileImageSource)AddProfilePic.Source;
        string employeePhoto = getNewImage.File;

        newEmployee.Add(new EmployeeInfo
        {
            ID = 6,
            FirstName = FirstNameAdd.Text,
            LastName = LastNameAdd.Text,
            Department = DepartmentAdd.Text,
            PhoneNumber = PhoneAdd.Text,
            AddressLine1 = AddressAdd1.Text,
            AddressLine2 = AddressAdd2.Text,
            ProfilePic = getNewImage
        });

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = false
        };

        var localFile = "/FILE/PATH/HERE/employee_local.csv";
        var tempFile = "/FILE/PATH/HERE/employee_temp.csv";

        using (Stream streamFile = await FileSystem.Current.OpenAppPackageFileAsync(localFile))
        using (var reader = new StreamReader(streamFile))
        using (var csv = new CsvReader(reader, config))
        using (var stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        using (var writer = new StreamWriter(stream))
        using (var csvWrite = new CsvWriter(writer, config))
        {
            while (csv.Read())
            {
                employeeList.Add(new EmployeeInfo
                {
                    ID = csv.GetField<int>(0),
                    FirstName = csv.GetField<string>(1),
                    LastName = csv.GetField<string>(2),
                    Department = csv.GetField<string>(3),
                    PhoneNumber = csv.GetField<string>(4),
                    AddressLine1 = csv.GetField<string>(5),
                    AddressLine2 = csv.GetField<string>(6),
                    ProfilePic = csv.GetField<string>(7)
                });
            }

            csvWrite.WriteRecords(employeeList);
            csvWrite.WriteRecords(newEmployee);
        }

        // Delete existing copy, replace with temp copy (renamed as existing copy)
        File.Delete(localFile);
        File.Move(tempFile, localFile);

        await DisplayAlert("New employee saved", "You have successfully added a new employee", "OK");

        await Navigation.PushAsync(new MainPage());
    }

    // Go back to staff directory
    private async void OnCancelButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}
