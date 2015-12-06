using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gdi = System.Drawing;
using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace keeperScoreboard.Classes
{
    public class ScoreboardRenderer
    {
        XAML.Scoreboard m_parent;
        public ScoreboardRenderer(XAML.Scoreboard parent, string mapID, CustomSnapshotRoot root, string teamlogo1 = "", string teamlogo2 = "")
        {
            ScoreboardRenderer1(parent, mapID, root, teamlogo1, teamlogo2);
        }
        public async void ScoreboardRenderer1(XAML.Scoreboard parent, string mapID, CustomSnapshotRoot root, string teamlogo1 = "", string teamlogo2 = "")
        {

            if ((root.snapshot.teamInfo.team1.player.Count + root.snapshot.teamInfo.team2.player.Count) > 10)
                return;
            string teamLogoDir = Directory.GetCurrentDirectory() + "\\images\\team_logos\\";
            string scoreboardTemplate = Directory.GetCurrentDirectory() + "\\images\\background\\bg.png";
            if(root == null)
            {
                return;
            }
            //Set up where to output the image to
            m_parent = parent;
            //Get the map image from resources Dynamically
            object O = Properties.Resources.ResourceManager.GetObject(mapID.ToLower());
            //Font type 
            Gdi.Font font = new Gdi.Font("Trebuchet MS", 20f, System.Drawing.FontStyle.Bold);
            //Load the scoreboard template image
            Gdi.Bitmap bitmap = null;
            try
            {
                bitmap = new Gdi.Bitmap(scoreboardTemplate);
            }catch(Exception)
            {
                bitmap = Properties.Resources.bg;
            }

            Gdi.Bitmap clone = new Gdi.Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Gdi.Graphics gr = Gdi.Graphics.FromImage(clone);
            
                gr.DrawImage(bitmap, new Gdi.Rectangle(0, 0, clone.Width, clone.Height));
            
            //bitmap.MakeTransparent();
            //bitmap.
            //Load the bitmap to a gdi graphic
            Gdi.Graphics g = Gdi.Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = Gdi.Text.TextRenderingHint.AntiAliasGridFit;
            Gdi.Bitmap team1Logo = null;
            Gdi.Bitmap team2Logo = null;
            //========= Team 1s Logo
            if (teamlogo1 == "")
            {
                if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) == "US")
                    team1Logo = new Gdi.Bitmap(Properties.Resources.usa);
                else if(Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) == "CN")
                    team1Logo = new Gdi.Bitmap(Properties.Resources.china);
                else if(Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) == "RU")
                    team1Logo = new Gdi.Bitmap(Properties.Resources.ru);
            }
            else
            {
                team1Logo = new Gdi.Bitmap(teamLogoDir + teamlogo1);

            }
            //Load the map image to a bitmap
            g.DrawImage(resize(250, 250, (Gdi.Image)team1Logo), 200, 360);
            //========= Team 2s Logo
            if (teamlogo2 == "")
            {
                if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) == "US")
                    team2Logo = new Gdi.Bitmap(Properties.Resources.usa);
                else if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) == "CN")
                    team2Logo = new Gdi.Bitmap(Properties.Resources.china);
                else if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) == "RU")
                    team2Logo = new Gdi.Bitmap(Properties.Resources.ru);
            }
            else
            {
                team2Logo = new Gdi.Bitmap(teamLogoDir + teamlogo2);
            }            //Load the map image to a bitmap
            g.DrawImage(resize(250, 250, (Gdi.Image)team2Logo), 1000, 360);


            Gdi.Bitmap map = new Gdi.Bitmap(resize(350, 170, (Gdi.Image)O));
            //draw the map image onto the scoreboard template
            g.DrawImage(map, 1200, 880);
            map.Dispose();
            map = null;
            //draw a string onto the map image
            g.DrawString(JSONHelper.whatMap(root.snapshot.mapId), font, Gdi.Brushes.White, 1480, 880);
            g.DrawString(JSONHelper.whatMode(root.snapshot.modeId), font, Gdi.Brushes.White, 1480, 910);
            g.DrawString(UsefulFunctions.getTime(root.snapshot.roundTime, 1), font, Gdi.Brushes.White, 1480, 940);

            //Draw player names for team 1 and team 2 with player data to the scoreboard
            //http://battlelog.battlefield.com/bf4/servers/show/pc/1ab24458-5ee9-4765-a2e8-fa96c464f506/Resistence-Team-Server-SQOBLITERATION-5on5-ESL-Rules-ON/
            int row = 560;
            int max = 58;
            int team1Kills = 0;
            int team1Deaths = 0;
            int team1Score = 0;
            g.DrawString(JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) + " - " + root.snapshot.team1Tickets.tickets.ToString() + "/" + root.snapshot.team1Tickets.ticketsMax.ToString()
                                                            , font, Gdi.Brushes.White, 200, 330);
            foreach (var player in root.snapshot.teamInfo.team1.player)
            {
                if (player.tag != "")
                {
                    string playerName = "[" + player.tag + "]" + player.name;
                    g.DrawString(playerName, font, Gdi.Brushes.White, 190, row);
                }
                else
                {
                    g.DrawString(player.name, font, Gdi.Brushes.White, 190, row);
                }
                //Draw Players Kit
                Gdi.Bitmap kitIcon = null;
                Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(player.playerId, player.name);

                switch (playerInfo.selectedKit.ToString())
                {
                    case "0":
                        kitIcon = Properties.Resources._0;
                        break;
                    case "1":
                        kitIcon = Properties.Resources._1;
                        break;
                    case "2":
                        kitIcon = Properties.Resources._2;
                        break;
                    case "3":
                        kitIcon = Properties.Resources._3;
                        break;
                    default:
                        kitIcon = Properties.Resources._0;
                        break;
                }
       
                g.DrawImage((Gdi.Image)kitIcon, 165, row);

                g.DrawString(player.score.ToString(), font, Gdi.Brushes.White, 480, row);
                g.DrawString(player.kills.ToString(), font, Gdi.Brushes.White, 620, row);
                g.DrawString(player.deaths.ToString(), font, Gdi.Brushes.White, 725, row);
                if (player.bombDetonationTime != null)
                {
                    int i = 1;
                    foreach (var time in player.bombDetonationTime)
                    {
                        g.DrawString(Classes.UsefulFunctions.getTime(time, 1), font, Gdi.Brushes.White, 850, (row + 35) - (i * 26));
                        i += 1;
                    }
                }
                row = row + max;
                team1Kills += player.kills;
                team1Deaths += player.deaths;
                team1Score += player.score;
                kitIcon.Dispose();

            }
            //------------------------------------
            //------------- Draw Team Totals
            g.DrawString(team1Score.ToString(), font, Gdi.Brushes.White, 480, 835);
            g.DrawString(team1Kills.ToString(), font, Gdi.Brushes.White, 620, 835);
            g.DrawString(team1Deaths.ToString(), font, Gdi.Brushes.White, 725, 835);

            //-------------------------------------
            row = 560;
            int rightOffset = 815;
            g.DrawString(JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) + " - " + root.snapshot.team2Tickets.tickets.ToString() + "/" + root.snapshot.team2Tickets.ticketsMax.ToString()
                                            , font, Gdi.Brushes.White, 200 + rightOffset, 330);

            int team2Kills = 0;
            int team2Deaths = 0;
            int team2Score = 0;
            foreach (var player in root.snapshot.teamInfo.team2.player)
            {
                if (player.tag != "")
                {
                    string playerName = "[" + player.tag + "]" + player.name;
                    g.DrawString(playerName, font, Gdi.Brushes.White, 190 + rightOffset, row);
                }
                else
                {
                    g.DrawString(player.name, font, Gdi.Brushes.White, 190 + rightOffset, row);
                }
                Gdi.Bitmap kitIcon = null;
                Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(player.playerId, player.name);

                switch (playerInfo.selectedKit.ToString())
                {
                    case "0":
                        kitIcon = Properties.Resources._0;
                        break;
                    case "1":
                        kitIcon = Properties.Resources._1;
                        break;
                    case "2":
                        kitIcon = Properties.Resources._2;
                        break;
                    case "3":
                        kitIcon = Properties.Resources._3;
                        break;
                }

                g.DrawImage((Gdi.Image)kitIcon, 165 + rightOffset, row);
                g.DrawString(player.score.ToString(), font, Gdi.Brushes.White, 480  + rightOffset, row);
                g.DrawString(player.kills.ToString(), font, Gdi.Brushes.White, 620 + rightOffset, row);
                g.DrawString(player.deaths.ToString(), font, Gdi.Brushes.White, 725+ rightOffset, row);
                if (player.bombDetonationTime != null)
                {
                    int i = 1;
                    foreach (var time in player.bombDetonationTime)
                    {
                        g.DrawString(Classes.UsefulFunctions.getTime(time, 1), font, Gdi.Brushes.White, 850 + rightOffset, (row + 35) - (i * 26));
                        i += 1;
                    }
                }
                row = row + max;
                team2Kills += player.kills;
                team2Deaths += player.deaths;
                team2Score += player.score;
                kitIcon.Dispose();
            }
            //------------------------------------
            //------------- Draw Team Totals
            g.DrawString(team2Score.ToString(), font, Gdi.Brushes.White, 480 + rightOffset, 835);
            g.DrawString(team2Kills.ToString(), font, Gdi.Brushes.White, 620 + rightOffset, 835);
            g.DrawString(team2Deaths.ToString(), font, Gdi.Brushes.White, 725 + rightOffset, 835);

            //-------------------------------------
            //Convert to ui readable image
            ImageSource bitmapSource = loadBitmap(bitmap);
            /*ImageSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                                  IntPtr.Zero,
                                  Int32Rect.Empty,
                                  BitmapSizeOptions.FromEmptyOptions());*/
            //Try and dispose everything
            if (bitmapSource != null)
            {
                m_parent.image.Source = bitmapSource;
            }
            font.Dispose();
            clone.Dispose();
            g.Dispose();
            bitmap.Dispose();
            team1Logo.Dispose();
            team2Logo.Dispose();
            gr.Dispose();
            
            // Create a writeable bitmap (which is a valid WPF Image Source
        }
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
        public Gdi.Image resize(int newWidth, int newHeight, Gdi.Image imgToResize)
        {
            Gdi.Bitmap b = new Gdi.Bitmap( newWidth, newHeight);
            Gdi.Graphics g = Gdi.Graphics.FromImage((Gdi.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, newWidth, newHeight);
            g.Dispose();
            imgToResize.Dispose();
            return (Gdi.Image)b;
        }

    }
}
