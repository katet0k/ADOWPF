using DBEntity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ADOWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=student_evaluations;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        

        Class1 databasseManager;
        public ObservableCollection<Student> Students { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            databasseManager = new Class1(connectionString);

            Students = new ObservableCollection<Student>();
            DG_Users.ItemsSource = Students;
        }

        private void Border_Click(object sender, MouseButtonEventArgs e)
        {
            ConnectToDatabase();
        }
        private void Border_Click2(object sender, MouseButtonEventArgs e)
        {
            DisplayInformation();
           
        }
        private void Border_Click3(object sender, MouseButtonEventArgs e)
        {
            DisplayAdditionalInformation();
        }
        private void ConnectToDatabase()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("Підключено до бази даних.");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка підключення до бази даних: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    MessageBox.Show("Відключено від бази даних.");
                }
            }
        }

        private void DisplayInformation()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                     
                    DisplayAllInformation(connection);
                     
                    DisplayStudentNames(connection);
                     
                    DisplayAverageGrades(connection);
                     
                    DisplayStudentsWithMinGrade(connection, 4.0);  
                     
                    DisplaySubjectsWithMinAverageGrade(connection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка виведення інформації: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void DisplayAllInformation(SqlConnection connection)
        {
            string query = "SELECT * FROM Students";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Students.Add(new Student
                    {
                        Fullname = reader["Fullname"].ToString(),
                        GroupName = reader["GroupName"].ToString(),
                        AverageGrade = Convert.ToDouble(reader["AverageGrade"]),
                        MinSubject = reader["MinSubject"].ToString(),
                        MaxSubject = reader["MaxSubject"].ToString()
                    });
                }
                reader.Close();
            }
        }

        private void DisplayStudentNames(SqlConnection connection)
        {
            string query = "SELECT Fullname FROM Students";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Students.Add(new Student { Fullname = reader["Fullname"].ToString() });
                }
                reader.Close();
            }
        }

        private void DisplayAverageGrades(SqlConnection connection)
        {
            string query = "SELECT Fullname, AverageGrade FROM Students";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Students.Add(new Student
                    {
                        Fullname = reader["Fullname"].ToString(),
                        AverageGrade = Convert.ToDouble(reader["AverageGrade"])
                    });
                }
                reader.Close();
            }
        }

        private void DisplayStudentsWithMinGrade(SqlConnection connection, double minGrade)
        {
            string query = $"SELECT Fullname FROM Students WHERE AverageGrade > {minGrade}";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Students.Add(new Student { Fullname = reader["Fullname"].ToString() });
                }
                reader.Close();
            }
        }

        private void DisplaySubjectsWithMinAverageGrade(SqlConnection connection)
        {
            string query = "SELECT DISTINCT MinSubject FROM Students WHERE MinSubject IS NOT NULL";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Students.Add(new Student { MinSubject = reader["MinSubject"].ToString() });
                }
                reader.Close();
            }
        }
        private void DisplayAdditionalInformation()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Очищення колекції перед додаванням нових даних
                    Students.Clear();


                    // Нові функції
                    DisplayMinAverageGrade(connection);
                    DisplayMaxAverageGrade(connection);
                    DisplayStudentsCountWithMinMathGrade(connection, "Math");
                    DisplayStudentsCountWithMaxMathGrade(connection, "Math");
                    DisplayStudentsCountInEachGroup(connection);
                    DisplayAverageGradePerGroup(connection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка виведення інформації: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void DisplayMinAverageGrade(SqlConnection connection)
        {
            string query = "SELECT MIN(AverageGrade) AS MinAverage FROM Students";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                double minAverage = Convert.ToDouble(command.ExecuteScalar());
                MessageBox.Show($"Мінімальна середня оцінка: {minAverage}");
            }
        }

        private void DisplayMaxAverageGrade(SqlConnection connection)
        {
            string query = "SELECT MAX(AverageGrade) AS MaxAverage FROM Students";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                double maxAverage = Convert.ToDouble(command.ExecuteScalar());
                MessageBox.Show($"Максимальна середня оцінка: {maxAverage}");
            }
        }

        private void DisplayStudentsCountWithMinMathGrade(SqlConnection connection, string subject)
        {
            string query = $"SELECT COUNT(*) FROM Students WHERE {subject} = (SELECT MIN({subject}) FROM Students)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                MessageBox.Show($"Кількість студентів з мінімальною оцінкою з {subject}: {count}");
            }
        }

        private void DisplayStudentsCountWithMaxMathGrade(SqlConnection connection, string subject)
        {
            string query = $"SELECT COUNT(*) FROM Students WHERE {subject} = (SELECT MAX({subject}) FROM Students)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                MessageBox.Show($"Кількість студентів з максимальною оцінкою з {subject}: {count}");
            }
        }

        private void DisplayStudentsCountInEachGroup(SqlConnection connection)
        {
            string query = "SELECT GroupName, COUNT(*) AS Count FROM Students GROUP BY GroupName";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show($"Кількість студентів у групі {reader["GroupName"]}: {reader["Count"]}");
                }
                reader.Close();
            }
        }

        private void DisplayAverageGradePerGroup(SqlConnection connection)
        {
            string query = "SELECT GroupName, AVG(AverageGrade) AS AvgGrade FROM Students GROUP BY GroupName";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show($"Середня оцінка у групі {reader["GroupName"]}: {reader["AvgGrade"]}");
                }
                reader.Close();
            }
        }
    }

public class Student
    {
        public string Fullname { get; set; }
        public string GroupName { get; set; }
        public double AverageGrade { get; set; }
        public string MinSubject { get; set; }
        public string MaxSubject { get; set; }
    }


}