using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexetchButBetter
{
    public partial class FriendlistHelperForm : Form
    {
        LeagueConnection lc;

        private bool alreadyScannedPlayers = false;
        private bool alreadyScannedClublist = false;

        private Dictionary<String, DateTime> playerLastMatchDateTime = new Dictionary<string, DateTime>();
        List<KeyValuePair<String, DateTime>> playerLastMatchDateTimeList;
        
        private Dictionary<String, String> clublist = new Dictionary<string, string>();
        
        public FriendlistHelperForm()
        {
            InitializeComponent();
        }

        private void FriendlistHelperForm_Load(object sender, EventArgs e)
        {
            lc = new LeagueConnection();
        }

        private async void loadFriends_Click(object sender, EventArgs e)
        {
            if (!lc.IsConnected)
            {
                MessageBox.Show("Not connected to LCU! Start the game and wait few seconds.");
                return;
            }

            if (alreadyScannedPlayers)
            {
                MessageBox.Show("Already scanned players");
                return;
            }
            else
            {
                alreadyScannedPlayers = true;
            }
            
            JsonArray friends = (JsonArray) await lc.Get("/lol-chat/v1/friends");
            clearOutputPanel();
            int i = 0;
            foreach (JsonObject friend in friends)
            {
                String name = (String) friend["name"];
                long id = (long) friend["summonerId"];
                JsonObject summoner = (JsonObject) await lc.Get("/lol-summoner/v1/summoners/" + id);
                long accountId = (long) summoner["accountId"];
                
                JsonObject matchlists = (JsonObject) await lc.Get("/lol-match-history/v1/friend-matchlists/" + accountId);
                JsonArray games = (JsonArray)((JsonObject)matchlists["games"])["games"];
                JsonObject game = (JsonObject) games[games.Count-1];
                String gameCreationDate = (String) game["gameCreationDate"];
                DateTime dateTime = DateTime.Parse(gameCreationDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
                playerLastMatchDateTime.Add(name, dateTime);
                
                Label label = new Label();
                label.Text = name + " " + accountId + " " + ++i + "/" + friends.Count;
                label.AutoSize = true;
                label.Margin = new Padding(5, 5, 5, 5);

                outputPanel.Controls.Add(label);
            }
            
            playerLastMatchDateTimeList = playerLastMatchDateTime.ToList();
            playerLastMatchDateTimeList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
            
            printData();
            
            MessageBox.Show("Done");
        }
        
        private void clearOutputPanel()
        {
            outputPanel.Controls.Clear();
        }

        private void printData()
        {
            clearOutputPanel();
            foreach (KeyValuePair<String, DateTime> friend in playerLastMatchDateTimeList)
            {
                Label label = new Label();
                label.Text = friend.Key + " " + friend.Value.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
                label.AutoSize = true;
                label.Margin = new Padding(5, 5, 5, 5);
                outputPanel.Controls.Add(label);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (!lc.IsConnected)
            {
                MessageBox.Show("Not connected to LCU! Start the game and wait few seconds.");
                return;
            }

            if (alreadyScannedClublist)
            {
                MessageBox.Show("Already scanned clublist");
                return;
            }
            else
            {
                alreadyScannedClublist = true;
            }
            
            JsonArray clubs = (JsonArray) await lc.Get("/lol-clubs/v1/clubs");
            foreach (JsonObject club in clubs)
            {
                String name = (String) club["name"];
                String key = (String) club["key"];
                clublist.Add(name, key);
                comboBox1.Items.Add(name); 
            }

            comboBox1.SelectedIndex = 0;
            
            MessageBox.Show("Done");
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (!lc.IsConnected)
            {
                MessageBox.Show("Not connected to LCU! Start the game and wait few seconds.");
                return;
            }
            if (!alreadyScannedClublist)
            {
                MessageBox.Show("Scan clublist first");
                return;
            }
            if (alreadyScannedPlayers)
            {
                MessageBox.Show("Already scanned players");
                return;
            }
            else
            {
                alreadyScannedPlayers = true;
            }
            String selected = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
            JsonArray members = (JsonArray) await lc.Get("/lol-clubs/v1/clubs/" + clublist[selected] + "/members");
            clearOutputPanel();
            int i = 0;
            foreach (JsonObject member in members)
            {
                String name = (String) member["summonerName"];
                long accountId = (long) member["accountId"];
                
                JsonObject matchlists = (JsonObject) await lc.Get("/lol-match-history/v1/friend-matchlists/" + accountId);
                JsonArray games = (JsonArray)((JsonObject)matchlists["games"])["games"];
                JsonObject game = (JsonObject) games[games.Count-1];
                String gameCreationDate = (String) game["gameCreationDate"];
                DateTime dateTime = DateTime.Parse(gameCreationDate, null, System.Globalization.DateTimeStyles.RoundtripKind);
                playerLastMatchDateTime.Add(name, dateTime);
                
                Label label = new Label();
                label.Text = name + " " + accountId + " " + ++i + "/" + members.Count;
                label.AutoSize = true;
                label.Margin = new Padding(5, 5, 5, 5);

                outputPanel.Controls.Add(label);
            }
            
            playerLastMatchDateTimeList = playerLastMatchDateTime.ToList();
            playerLastMatchDateTimeList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
            
            printData();
            
            MessageBox.Show("Done");
        }
    }
}