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
            JsonObject chatMe = await lc.Get("/lol-chat/v1/me");
            String fromId = (String) chatMe["puuid"];
            long fromSummonerId = (long) chatMe["summonerId"];
            JsonArray friends = (JsonArray) await lc.Get("/lol-chat/v1/friends");
            clearOutputPanel();
            String msg = messageTextbox.Text;
            int i = 0;
            foreach (JsonObject friend in friends)
            {
                await Task.Delay(3000);
                Player player = new Player();
                player.name = (String) friend["name"];
                player.riotName = (String) friend["gameName"];
                player.riotTag = (String) friend["gameTag"];
                player.lastMatch = DateTime.MaxValue;
                
                String friendId = (String) friend["pid"];

                try
                {
                    await sendMessage(fromId, fromSummonerId, friendId, msg);
                }
                catch (Exception exception)
                {
                    printPlayer(player, " error sending msg");
                }
                
                /*
                long id = (long) friend["summonerId"];
                if (id != 0)
                {
                    JsonObject summoner = (JsonObject) await lc.Get("/lol-summoner/v1/summoners/" + id);
                    long accountId = (long) summoner["accountId"];
                    player.lastMatch = await getPlayerData(accountId);
                }
                players.Add(player);
                */
                
                printPlayer(player, " " + ++i + "/" + friends.Count);
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
            String text = player.name;
            
            /*
            if (player.name != null && player.riotName != null) text += player.name + " ";
            text += player.lastMatch.ToString("yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);
            */
            
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

        private async Task sendMessage(String fromId, long fromSummonerId, String friendId, String msg)
        {
            String timestamp = DateTime.UtcNow.ToString("s") + "Z";
            
            await lc.Post("/lol-chat/v1/conversations/" + friendId + "/messages",
                "{\"type\":\"chat\",\"fromId\":\"" + fromId + "\",\"fromSummonerId\":" + fromSummonerId + ",\"isHistorical\":false,\"timestamp\":\"" + timestamp + "\",\"body\":\"" + msg + "\"}");
            
            //Trace.WriteLine("Sending msg " + msg + " to " + friendId);
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