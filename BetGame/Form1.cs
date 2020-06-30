using BetGame.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetGame
{
    public partial class Form1 : Form
    {
        Greyhound[] cars= new Greyhound[4];
        Punter[] punters = new Punter[3];
        Greyhound winnerCar;
        int stop;

        Timer[] timers = new Timer[4];
        public Form1()
        {
            InitializeComponent();
            PrepareInitialData();
            SetBet();
            btnGameOver.Enabled = false;
            btnStartRace.Enabled = false;
        }

        private void PrepareInitialData()
        {
            cars[0] = new Greyhound() { CarName = "Car 1", RaceTrackLength = 970, MyPictureBox = pictureCar1 };
            cars[1] = new Greyhound() { CarName = "Car 2", RaceTrackLength = 970, MyPictureBox = pictureCar2 };
            cars[2] = new Greyhound() { CarName = "Car 3", RaceTrackLength = 970, MyPictureBox = pictureCar3 };
            cars[3] = new Greyhound() { CarName = "Car 4", RaceTrackLength = 970, MyPictureBox = pictureCar4 };
            punters[0] = Factory.GetAPunter(1);
            punters[1] = Factory.GetAPunter(2);
            punters[2] = Factory.GetAPunter(3);
            punters[0].MyLabel = lblBets;
            punters[0].MyRadioButton = radioPunter1;
            punters[0].MyText = txtPunter1;
            punters[1].MyLabel = lblBets;
            punters[1].MyRadioButton = radioPunter2;
            punters[1].MyText = txtPunter2;
            punters[2].MyLabel = lblBets;
            punters[2].MyRadioButton = radioPunter3;
            punters[2].MyText = txtPunter3;
            punters[0].MyRadioButton.Text = punters[0].Name;
            punters[1].MyRadioButton.Text = punters[1].Name;
            punters[2].MyRadioButton.Text = punters[2].Name;
            numericCarNumber.Minimum = 1;
            numericCarNumber.Maximum = 4;
            numericCarNumber.Value = 1;
        }
       

        private void SetBet()
        {
            foreach(Punter punter in punters )
            {
                if (punter.Busted)
                {
                    punter.MyText.Text = "BUSTED";
                }
                else
                {
                    if (punter.MyBet == null)
                    {
                        punter.MyText.Text = punter.Name + " hasn't placed a bet";
                    }
                    else
                    {
                        punter.MyText.Text = punter.Name + " bets $" + punter.MyBet.Amount + " on " + punter.MyBet.Car.CarName;
                    }
                    if (punter.MyRadioButton.Checked)
                    {
                        lblMaxBet.Text = "Max Bet is $" + punter.Cash.ToString();
                        btnPlaceBet.Text = "Place Bet for " + punter.Name;
                        numericBetAmount.Minimum = 1;
                        numericBetAmount.Maximum = punter.Cash;
                        numericBetAmount.Value = 1;
                    }
                }
            }
        }

      
        private void radioPunter_CheckedChanged(object sender, EventArgs e)
        {
            SetBet();
        }

        private void btnPlaceBet_Click(object sender, EventArgs e)
        {
            int count = 0;
            int total_active = 0;
            foreach(Punter punter in punters)
            {
                if(!punter.Busted)
                {
                    total_active++;
                }
                if(punter.MyRadioButton.Checked)
                {
                    if( punter.MyBet == null )
                    {
                        int number = (int)numericCarNumber.Value;
                        int amount = (int)numericBetAmount.Value;
                        bool alreadyPlaced = false;
                        foreach(Punter pun in punters)
                        {
                            if( pun.MyBet != null && pun.MyBet.Car == cars[number-1])
                            {
                                alreadyPlaced = true;
                                break;
                            }
                        }
                        if (alreadyPlaced)
                        {
                            MessageBox.Show("This Car Number is Already Taken. Try With Different Car");
                        }
                        else
                        {
                            punter.MyBet = new Bet() { Amount = amount, Car = cars[number - 1] };
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("You Already Place Bet for " + punter.Name);
                    }
                }
                if (punter.MyBet != null)
                {
                    count++;
                }
            }
            if( count == total_active)
            {
                btnPlaceBet.Enabled = false;
                btnStartRace.Enabled = true;
            }
            SetBet();
        }

        private void btnStartRace_Click(object sender, EventArgs e)
        {
            for(int index = 0; index < timers.Length; index++)
            {
                timers[index] = new Timer();
                timers[index].Interval = 15;
                timers[index].Tick += Timer_Tick;
                timers[index].Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (sender is Timer)
            {
                Timer timer = sender as Timer;
                int index = 0;
                while( index < timers.Length)
                {
                    if(timers[index] == timer)
                    {
                        break;
                    }
                    index++;
                }
                PictureBox picture = cars[index].MyPictureBox;
                if(picture.Location.X + picture.Width > cars[index].RaceTrackLength)
                {
                    timer.Stop();
                    stop++;
                    if(winnerCar==null)
                    {
                        winnerCar = cars[index];
                    }                    
                }
                else
                {
                    int jump = new Random().Next(1,15);
                    picture.Location = new Point(picture.Location.X + jump, picture.Location.Y);
                }
                
            }
            if (stop == timers.Length)
            {
                MessageBox.Show(winnerCar.CarName + " is Won!!!");
                SetBet();
                foreach (Punter punter in punters)
                {
                    if (punter.MyBet != null)
                    {
                        if (punter.MyBet.Car == winnerCar)
                        {
                            punter.Cash += punter.MyBet.Amount;
                            punter.MyText.Text = punter.Name + " Won and now has $" + punter.Cash;
                            punter.Winner = true;
                        }
                        else
                        {
                            punter.Cash -= punter.MyBet.Amount;
                            if( punter.Cash == 0 )
                            {
                                punter.MyText.Text = "BUSTED";
                                punter.Busted = true;
                                punter.MyRadioButton.Enabled = false;
                            }
                            else
                            {
                                punter.MyText.Text = punter.Name + " Lost and now has $" + punter.Cash;
                            }                            
                        }
                    }
                }
                winnerCar = null;
                stop = 0;
                timers = new Timer[4];
                btnPlaceBet.Enabled = true;
                btnStartRace.Enabled = false;
                int count = 0;
                foreach(Punter punter in punters)
                {
                    if (punter.Busted)
                    {
                        count++;
                    }
                    if (punter.MyRadioButton.Enabled && punter.MyRadioButton.Checked)
                    {
                        lblMaxBet.Text = "Max Bet is $" + punter.Cash;
                    }
                    punter.MyBet = null;
                    punter.Winner = false;
                }
                if(count==punters.Length)
                {
                    btnPlaceBet.Enabled = false;
                    btnGameOver.Enabled = true;
                }
                foreach(Greyhound car in cars)
                {
                    car.MyPictureBox.Location = new Point(1,car.MyPictureBox.Location.Y);
                }
            }
        }

        private void btnGameOver_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bye Bye From Game");
            Application.Exit();
        }
    }
}
