using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Maui.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace MobileApp;

public partial class UpdateDetails : ContentPage
{
    public List<EmployeeInfo> currentEmployee = new();
    public List<EmployeeInfo> employeeList = new();

    public string employeeID;
    public string currentPicture;

    // Get params passed from EmployeeDetails page, set employee details view as params
    public UpdateDetails(params string[] data)
	{
		InitializeComponent();

        employeeID = data[0];

        FirstNameUpdate.Text = data[1];
        LastNameUpdate.Text = data[2];
        DepartmentUpdate.Text = data[3];
        PhoneUpdate.Text = data[4];
        AddressUpdateLine1.Text = data[5];
        AddressUpdateLine2.Text = data[6];
        PictureUpdate.Source = data[7];

        currentPicture = data[7];
    }

    // Find employee in employee.csv, remove from list, add updated employee, rewrite file
    private async void OnSaveButtonClicked(object sender, EventArgs e)
	{
        currentEmployee.Add(new EmployeeInfo
        {
            ID = Convert.ToInt32(employeeID),
            FirstName = FirstNameUpdate.Text,
            LastName = LastNameUpdate.Text,
            Department = DepartmentUpdate.Text,
            PhoneNumber = PhoneUpdate.Text,
            AddressLine1 = AddressUpdateLine1.Text,
            AddressLine2 = AddressUpdateLine2.Text,
            ProfilePic = currentPicture
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

                foreach (var entry in employeeList)
                {
                    if (entry.ID == Convert.ToInt32(employeeID))
                    {
                        employeeList.Remove(entry);
                        break;
                    }
                }
            }

            csvWrite.WriteRecords(employeeList);
            csvWrite.WriteRecords(currentEmployee);
        }

        File.Delete(localFile);
        File.Move(tempFile, localFile);

        await DisplayAlert("Details saved", "You have successfully updated the employee records", "OK");

        await Navigation.PushAsync(new MainPage());
    }
}