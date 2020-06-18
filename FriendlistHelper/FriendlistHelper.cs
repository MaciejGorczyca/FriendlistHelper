using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexetchButBetter
{
    public partial class FriendlistHelperForm : Form
    {
        LeagueConnection lc;

        private bool alreadyScannedPlayers = false;
        private bool alreadyScannedClublist = false;

        List<Player> players = new List<Player>();
        
        private Dictionary<String, String> clublist = new Dictionary<string, string>();

        private const int MARGIN_SIZE = 5;
        private const int LABEL_WIDTH = 438;
        private const int LABEL_HEIGHT = 13;
        private const int TEXTBOX_WIDTH = 100;
        
        
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
            alreadyScannedPlayers = true;
            JsonArray friends = (JsonArray) await lc.Get("/lol-chat/v1/friends");
            clearOutputPanel();
            int i = 0;
            foreach (JsonObject friend in friends)
            {
                Player player = new Player();
                player.name = (String) friend["name"];
                player.riotName = (String) friend["gameName"];
                player.riotTag = (String) friend["gameTag"];
                player.lastMatch = DateTime.MaxValue;
                long id = (long) friend["summonerId"];
                if (id != 0)
                {
                    JsonObject summoner = (JsonObject) await lc.Get("/lol-summoner/v1/summoners/" + id);
                    long accountId = (long) summoner["accountId"];
                    player.lastMatch = await getPlayerData(accountId);
                }
                players.Add(player);
                printPlayer(player, " " + ++i + "/" + friends.Count);
            }
            printData();
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
            alreadyScannedPlayers = true;
            String selected = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
            JsonArray members = (JsonArray) await lc.Get("/lol-clubs/v1/clubs/" + clublist[selected] + "/members");
            clearOutputPanel();
            int i = 0;
            foreach (JsonObject member in members)
            {
                Player player = new Player();
                player.name = (String) member["summonerName"];
                long accountId = (long) member["accountId"];
                player.lastMatch = await getPlayerData(accountId);
                players.Add(player);
                printPlayer(player, " " + ++i + "/" + members.Count);
            }
            printData();
        }
        
        
        private void clearOutputPanel()
        {
            outputPanel.Controls.Clear();
        }

        private void printData()
        {
            players = players.OrderBy(p=>p.lastMatch).ToList();
            clearOutputPanel();
            foreach (Player player in players)
            {
                printPlayer(player);
            }
            MessageBox.Show("Done");
        }

        private Label prepareLabel()
        {
            Label label = new Label();
            label.AutoSize = false;
            label.Margin = new Padding(MARGIN_SIZE, MARGIN_SIZE, MARGIN_SIZE, MARGIN_SIZE);
            label.Size = new Size(LABEL_WIDTH - (TEXTBOX_WIDTH+MARGIN_SIZE+1) - MARGIN_SIZE * 2 - 17, LABEL_HEIGHT);
            return label;
        }

        private String prepareLabelString(Player player)
        {
            String text = "";
            if (player.name != null && player.riotName != null) text += player.name + " ";
            text += player.lastMatch.ToString("yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);
            return text;
        }

        private void printPlayer(Player player)
        {
            printPlayer(player, "");
        }

        private void printPlayer(Player player, String appendText)
        {
            TextBox textBox = new TextBox();
            if (player.riotName != null) textBox.Text = player.riotName + "#" + player.riotTag;
            else textBox.Text = player.name;
            outputPanel.Controls.Add(textBox);

            Label label = prepareLabel();
            label.Text = prepareLabelString(player) + appendText;
            outputPanel.Controls.Add(label);
        }

        private async Task<DateTime> getPlayerData(long accountId)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    JsonObject matchlists = (JsonObject) await lc.Get("/lol-match-history/v1/friend-matchlists/" + accountId);
                    JsonArray games = (JsonArray) ((JsonObject) matchlists["games"])["games"];
                    if (games.Count == 0) return DateTime.MinValue;
                    JsonObject game = (JsonObject) games[games.Count - 1];
                    String gameCreationDate = (String) game["gameCreationDate"];
                    return DateTime.Parse(gameCreationDate, null, DateTimeStyles.RoundtripKind);
                }
                catch (Exception exception)
                {
                    if (++i > 3)    // the game occasionally throws error for some reason, retry up to 3 times
                    {
                        MessageBox.Show("Error retrieving matchlist data for player id " + accountId);
                        return DateTime.MinValue;
                    }
                    Trace.WriteLine("Error while getting matchlist data, retrying");
                    Thread.Sleep(500);
                }
            }
        }
    }
}
    
public class Player
{
    public string name { get; set; }
    public string riotName { get; set; }
    public string riotTag { get; set; }
    public DateTime lastMatch { get; set; }
}