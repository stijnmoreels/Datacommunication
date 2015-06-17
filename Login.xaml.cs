using nmct.datacom.colordome.businesslayer;
using nmct.datacom.colordome.businesslayer.Services;
using nmct.datacom.colordome.model;
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
using System.Windows.Shapes;
using nmct.datacom.colordome.businesslayer.Repositories;
using nmct.datacom.colordome.businesslayer.Context;
using System.Windows.Interop;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using nmct.datacom.colordome.hardware;

namespace nmct.datacom.colordome.wpf
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
        private IColorDomeService _service = null;

        public Login(IColorDomeService service)
        {
            _service = service;
            InitializeComponent();
            txtCode1.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginProcess();
        }

        private void LoginProcess()
        {
            // MICHIEL -> nog waarde Smartcard lezen
            string value = GetValueOnCard();
            if (value != null)
            {
                String[] splitWaarde = value.Split('.');
                UserSmartCard user = _service.FindUserById(splitWaarde[0]);
                if (user != null && user.Access == true)
                {
                    SaveLogin(splitWaarde,user);
                    OpenMainWindow(user);
                }
                else
                    txtError.Text = "U hebt geen toegang tot de camera of bent \n nog niet toegevoegd aan het systeem";
            }
        }

        private void SaveLogin(String[] splitWaarde, UserSmartCard user)
        {
            SaveLoginOnCard(splitWaarde);
            SaveLoginOnDatabase(user);
        }

        private void SaveLoginOnDatabase(UserSmartCard user)
        {
            user.LastLogin = DateTime.Now;
            _service.UpdateLastLoginUser(user);
        }

        private void SaveLoginOnCard(string[] splitWaarde)
        {
            String waardeOpKaart = String.Format("{0}.{1}.{2}.{3}", splitWaarde[0], splitWaarde[1], splitWaarde[2], DateTime.Now.ToString());
            card.WriteDataSecure(waardeOpKaart);
        }

        private void OpenMainWindow(UserSmartCard user)
        {
            /*RegisterNewUser form = new RegisterNewUser(_service);
            WindowInteropHelper wih = new WindowInteropHelper(this);
            wih.Owner = form.Handle;
            this.Close();
            form.ShowDialog();*/

            MainWindow window = new MainWindow(_service);
            window.User = user;
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = window.Handle;
            this.Close();
            window.ShowDialog();
        }
        SmartCard card;
        private string GetValueOnCard()
        {
            card = new SmartCard();
            if (card.Readers == null)
                return CardReaderError("De cardReader is niet goed aangesloten");
            Boolean connect = card.ConnectSecure(card.Readers[0]);
            if (!connect)
                return CardReaderError("De cardReader is niet goed aangesloten");
            Boolean code = card.CheckCode(getCode());
            if(! code)
                return CardReaderError("U heeft de verkeerde code ingegeven");
            return card.ReadDataSecure();
        }

        private byte[] getCode()
        {
            byte code1 = Convert.ToByte(txtCode1.Text);
            byte code2 = Convert.ToByte(txtCode2.Text);
            byte code3 = Convert.ToByte(txtCode3.Text);
            return new byte[3] { code1, code2, code3 };
        }

        private string CardReaderError(string text)
        {
            txtError.Text = text;
            return null;
        }

        private void txtCode1_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtCode1.Text.Length == 3)
                txtCode2.Focus();
        }

        private void txtCode2_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtCode2.Text.Length == 3)
                txtCode3.Focus();
        }

        private void txtCode3_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtCode3.Text.Length == 3)
                btnLogin.Focus();
        }
    }
}
