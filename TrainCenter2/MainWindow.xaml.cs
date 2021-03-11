using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;


namespace TrainCenter2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        myTraincenterEntities context = new myTraincenterEntities();
        CollectionViewSource atletiViewSource;
        CollectionViewSource seduteViewSource;

        public MainWindow()
        {
            InitializeComponent();
            atletiViewSource = ((CollectionViewSource)(FindResource("atletiViewSource")));
            seduteViewSource = ((CollectionViewSource)(FindResource("atletiSeduteViewSource")));
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //System.Windows.Data.CollectionViewSource seduteViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("seduteViewSource")));
            // Charger les données en définissant la propriété CollectionViewSource.Source :
            // seduteViewSource.Source = [source de données générique]
            //System.Windows.Data.CollectionViewSource atletiViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("atletiViewSource")));
            // Charger les données en définissant la propriété CollectionViewSource.Source :
            // atletiViewSource.Source = [source de données générique]
            context.Atleti.Load();
            // After the data is loaded, call the DbSet<T>.Local property    
            // to use the DbSet<T> as a binding source.   
            atletiViewSource.Source = context.Atleti.Local;
         

        }

        private void LastCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            atletiViewSource.View.MoveCurrentToLast();
        }

        private void PreviousCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            atletiViewSource.View.MoveCurrentToPrevious();
        }

        private void NextCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            atletiViewSource.View.MoveCurrentToNext();
        }

        private void FirstCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            atletiViewSource.View.MoveCurrentToFirst();
        }



        private void DeleteSedutaCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // Get the sedute in the row in which the Delete button was clicked.  
            Sedute obj = e.Parameter as Sedute;
            Delete_Sedute(obj);
        }

        private void Delete_Sedute(Sedute sedute)
        {
            // Find the sedute in the EF model.  
            var ord = (from o in context.Sedute.Local
                       where o.SedutaID == sedute.SedutaID
                       select o).FirstOrDefault();
            
            //TODO Check if 
            // Delete all the sedute_details that have  
            // this sedute as a foreign key  
           
           // foreach (var detail in ord.atleta_sedute.ToList())
            //{
             //   context.sedute_Details.Remove(detail);
           // }

            // Now it's safe to delete the sedute.  
            context.Sedute.Remove(ord);
            context.SaveChanges();

            // Update the data grid.  
            seduteViewSource.View.Refresh();
        }



        // Cancels any input into the new atleta form  
        private void CancelCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            add_atletaIDTextBox.Text = "";
            add_nomeTextBox.Text = "";
            add_cognomeTextBox.Text = "";
            add_pesoTextBox.Text = "";
            add_altezzaTextBox.Text = "";
            //add_fotoTextBox.Text = "";

            existingAtletaGrid.Visibility = Visibility.Visible;
            newAtletaGrid.Visibility = Visibility.Collapsed;
            newSedutaGrid.Visibility = Visibility.Collapsed;
        }


        private void NewSeduta_click(object sender, RoutedEventArgs e)
        {
            var cust = atletiViewSource.View.CurrentItem as Atleti;
            if (cust == null)
            {
                MessageBox.Show("No atleta selected.");
                return;
            }
            newSedutaGrid.Visibility = Visibility.Visible;
            existingAtletaGrid.Visibility = Visibility.Collapsed;
            newAtletaGrid.Visibility = Visibility.Collapsed;
            newSedutaGrid.UpdateLayout();
           
        }



        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        private void AddCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            existingAtletaGrid.Visibility = Visibility.Collapsed;
            newSedutaGrid.Visibility = Visibility.Collapsed;
            newAtletaGrid.Visibility = Visibility.Visible;

            // Clear all the text boxes before adding a new customer.  
            foreach (var child in newAtletaGrid.Children)
            {
                var tb = child as TextBox;
                if (tb != null)
                {
                    tb.Text = "";
                }
            }
        }



        // Commit changes from the new atleta form, the new seduta form,  
        // or edits made to the existing atleta form.  
        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (newAtletaGrid.IsVisible)
            {
                // Create a new object because the old one  
                // is being tracked by EF now.  
                Atleti newAtleta = new Atleti
                {
                    AtletaID = Int32.Parse(add_atletaIDTextBox.Text),
                    Nome = add_nomeTextBox.Text,
                    Cognome = add_cognomeTextBox.Text,
                  
                    DataDiNascita = add_dataDiNascitaDatePicker.SelectedDate,
                  
                   
                };  

                try
                {
                    newAtleta.peso = Convert.ToDecimal(add_pesoTextBox.Text);
                    newAtleta.Altezza = Convert.ToDecimal(add_altezzaTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Verificare che peso e altezza siano numeri decimali.");
                    return;
                }
                // Perform very basic validation  
                //if (newAtleta.CustomerID.Length == 5)
                //{
                // Insert the new customer at correct position:  
                int len = context.Atleti.Local.Count();
                    int pos = len;
                    for (int i = 0; i < len; ++i)
                    {

                        //TODO Verifier si correct
                        if (newAtleta.AtletaID > context.Atleti.Local[i].AtletaID)
                        {
                            pos = i+1;
                            break;
                        }
                    }
                    context.Atleti.Local.Insert(pos, newAtleta);
                    atletiViewSource.View.Refresh();
                    atletiViewSource.View.MoveCurrentTo(newAtleta);
                //}
            

                newAtletaGrid.Visibility = Visibility.Collapsed;
                existingAtletaGrid.Visibility = Visibility.Visible;
            }
            else if (newSedutaGrid.IsVisible)
            {
                // Order ID is auto-generated so we don't set it here.  
                // For CustomerID, address, etc we use the values from current customer.  
                // User can modify these in the datagrid after the order is entered.  

                Atleti currentAtleta = (Atleti)atletiViewSource.View.CurrentItem;

                Sedute newSeduta = new Sedute()
                {
                    Data = (DateTime)add_dataDatePicker.SelectedDate,
                    OraFine = TimeSpan.Parse(add_oraFineTextBox.Text),
                    OraInizio = TimeSpan.Parse(s: add_oraInizioTextBox.Text),  
                    AtletaID = currentAtleta.AtletaID,
                    SedutaID = add_sedutaIDTextBox.Text
                };
           
                try
                {
                    newSeduta.SportID = add_sportIDTextBox.Text;
                }
                catch
                {
                    MessageBox.Show("Il codice del vostro sport ha 5 caratteri.");
                    return;
                }
                //context.Sedute.Local.Insert(currentAtleta.AtletaID, newSeduta);

                // Add the oseduta into the EF model  
                context.Sedute.Add(newSeduta);
                seduteViewSource.View.Refresh();
                
            }

            // Save the changes, either for a new atleta, a new seduta 
            // or an edit to an existing atleta or order.
            context.SaveChanges();
            existingAtletaGrid.Visibility = Visibility.Visible;
            newAtletaGrid.Visibility = Visibility.Collapsed;
            newSedutaGrid.Visibility = Visibility.Collapsed;
        }

        private void DeleteAtletaCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  
            var cur = atletiViewSource.View.CurrentItem as Atleti;

            var cust = (from c in context.Atleti
                        where c.AtletaID == cur.AtletaID
                        select c).FirstOrDefault();

            if (cust != null)
            {
                foreach (var sedu in cust.Sedute.ToList())
                {
                    Delete_Sedute(sedu);
                }
                context.Atleti.Remove(cust);
            }
            context.SaveChanges();
            atletiViewSource.View.Refresh();
        }

        private void seduteDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
