using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gdi = System.Drawing;
using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

namespace keeperScoreboard.Classes
{
    class ScoreboardCompetitiveTeamRenderer
    {
        private XAML.TeamScoreboard m_parent;

        public ScoreboardCompetitiveTeamRenderer(XAML.TeamScoreboard parent, List<playersData> TeamData, string TeamName)
        {
            m_parent = parent;
            m_parent.image.Source = null;

            //set the current directory and filename for the template
            string scoreboardTemplate = Directory.GetCurrentDirectory() + "\\images\\background\\team_bg.png";
            string teamLogoDirectory = Directory.GetCurrentDirectory() + "\\images\\team_logos\\";
            string fontLocation = Directory.GetCurrentDirectory() + "\\fonts\\orbitron-black.ttf";
            // 'PrivateFontCollection' is in the 'System.Drawing.Text' namespace            
            var foo = new Gdi.Text.PrivateFontCollection();
            // Provide the path to the font on the filesystem
            foo.AddFontFile(fontLocation);
            //Make variable for the custom font
            var myCustomFont = new Gdi.Font((Gdi.FontFamily)foo.Families[0], 24f);
            Gdi.Bitmap bitmap = null;
            //try and load the background as a bitmap
            try
            {
                bitmap = new Gdi.Bitmap(scoreboardTemplate);
            }
            catch (Exception)
            {
                bitmap = Properties.Resources.bg;
            }
            //Clone the bitmap for nicer transparancy
            Gdi.Bitmap clone = new Gdi.Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            //convert the bitmap to a graphic
            Gdi.Graphics gr = Gdi.Graphics.FromImage(clone);
            //draw the bitmap the the graphic
            gr.DrawImage(bitmap, new Gdi.Rectangle(0, 0, clone.Width, clone.Height));

            //Load the bitmap to a gdi graphic and add some nice antialisasing to it
            Gdi.Graphics g = Gdi.Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = Gdi.Text.TextRenderingHint.AntiAliasGridFit;

            g.DrawString("Team Stats", myCustomFont, Gdi.Brushes.White, 230, 350);
            g.DrawString(TeamName, myCustomFont, Gdi.Brushes.White,  1500, 350);

            //Start drawing graphics for team members
            int rightOffset = 320;
            int heightOffset = 330;
            Gdi.StringFormat format = new Gdi.StringFormat();
            format.LineAlignment = Gdi.StringAlignment.Center;
            format.Alignment = Gdi.StringAlignment.Center;

            foreach (var player in TeamData)
            {
                bool renderOrNot = true;
                foreach( var team in MainWindow.CompetitiveTeamData)
                {
                    if(TeamName == team.TeamName)
                    {
                        foreach(var players in team.TeamMembers)
                        {
                            if (player.name == players.MemberName && players.MemberIsCoreOrSub == Structs.TeamMember.CoreOrSub.substitute)
                            {
                                renderOrNot = false;
                            }

                        }
                    }
                }
                if (renderOrNot == false)
                    continue;
                //Draw player name
                g.DrawString(player.name, myCustomFont, Gdi.Brushes.White, rightOffset, 150f + heightOffset, format);
                heightOffset += 72;
                //Draw player kills
                g.DrawString(player.kills.ToString(), myCustomFont, Gdi.Brushes.White, rightOffset, 150f + heightOffset, format);
                heightOffset += 72;
                //Draw player deaths
                g.DrawString(player.deaths.ToString(), myCustomFont, Gdi.Brushes.White, rightOffset, 150f + heightOffset, format);
                heightOffset += 72;
                //Draw player kdr
                g.DrawString((Math.Round((decimal)player.kills / (decimal)player.deaths, 2)).ToString(), myCustomFont, Gdi.Brushes.White, rightOffset, 150f + heightOffset, format);
                heightOffset += 72;
                //draw bombs detonated
                g.DrawString(player.score.ToString(), myCustomFont, Gdi.Brushes.White, rightOffset, 150f + heightOffset, format);

                heightOffset += 72;
                //draw score
                g.DrawString(player.bombsDetonated.ToString(), myCustomFont, Gdi.Brushes.White, rightOffset, 150f + heightOffset, format);

                rightOffset += 320;
                heightOffset = 330;
            }



            //Convert to ui readable image
            ImageSource bitmapSource = loadBitmap(bitmap);
            m_parent.image.Source = bitmapSource;
            clone.Dispose();
            g.Dispose();
            bitmap.Dispose();
            foo.Dispose();
            gr.Dispose();
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
    }
}
