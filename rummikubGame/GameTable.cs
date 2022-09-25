﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rummikubGame
{
    public partial class GameTable : Form
    {
        // consts
        public const int COMPUTER_PLAYER_TURN = 0;
        public const int HUMAN_PLAYER_TURN = 1;

        public static Form GameTableContext; // used in order to add buttons from other classes
        public static Pool pool; // the pool of cards
        public static Button dropped_tiles; // dropped_tiles button, used in the mouseUp

        public HumanPlayer humanPlayer; // human-player
        public static ComputerPlayer ComputerPlayer; // computer-player
        public static int current_turn;

        public GameTable()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GameTableContext = this; // updates the gameTable context
            dropped_tiles = dropped_tiles_btn; // updates the dropped_tiles variable, so it'll be accessed outside that class

            // Generate pool cards
            pool = new Pool(); // create pool object
            humanPlayer = new HumanPlayer("Player Default Name"); // create the humanPlayer object
            ComputerPlayer = new ComputerPlayer();

            // change the style of the drop_TileButtons_location
            dropped_tiles_btn.FlatStyle = FlatStyle.Flat;
            dropped_tiles_btn.FlatAppearance.BorderSize = 0;
            dropped_tiles_btn.BackColor = System.Drawing.ColorTranslator.FromHtml("#383B9A");

            // set round corners of the board
            board_panel.BackColor = System.Drawing.ColorTranslator.FromHtml("#383B9A");

            // set background color
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#383B9A");

            // set background color of pool btn
            pool_btn.BackColor = System.Drawing.ColorTranslator.FromHtml("#383B9A");
            pool_btn.FlatStyle = FlatStyle.Flat;
            pool_btn.FlatAppearance.BorderSize = 0;

            // set button flat design
            SortByColor_btn.FlatStyle = FlatStyle.Flat;
            SortByColor_btn.FlatAppearance.BorderSize = 0;
            SortByValue_btn.FlatStyle = FlatStyle.Flat;
            SortByValue_btn.FlatAppearance.BorderSize = 0;            

            // this will send back the panel(the board)
            board_panel.SendToBack();
            developerData();
            updatePoolSizeText();

            // the current turn
            /*
            Random rnd = new Random();
            int current_turn = rnd.Next(2);
            if (current_turn == COMPUTER_PLAYER_TURN)
                current_turn = COMPUTER_PLAYER_TURN;
            else
                current_turn = HUMAN_PLAYER_TURN;
            */
            current_turn = HUMAN_PLAYER_TURN; // CHEAT
        }

        private void updatePoolSizeText()
        {
            current_pool_size.Text = pool.getPoolSize() + " tiles in pool"; // updates the current tiles in the queue
        }

        public static bool checkWinner(List<List<Tile>> melds) 
        {
            // now var-melds has all the melds
            // if all the melds in the list are fine, the user won
            for (int i = 0; i < melds.Count(); i++)
            {
                if (!GameTable.isLegalMeld(melds[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool isLegalMeld(List<Tile> meld)
        {
            /*
             Legal meld, can be:
                - group(same number - different colors)
                - run(same color - different numbers(accending order))
             */

            if (meld.Count() < 3)
                return false;

            bool isRun = true;
            int color = meld[0].getColor();
            int value = meld[0].getNumber();

            for (int i = 1; i < meld.Count(); i++)
            {
                if (meld[i].getNumber() != value + i || meld[i].getColor() != color)
                {
                    isRun = false; break;
                }
            }
            if (isRun) return true;
            if (meld.Count() > 4) return false;

            for (int i = 0; i < meld.Count() - 1; i++)
            {
                if (meld[i + 1].getNumber() != value)
                    return false;
                for (int j = i + 1; j < meld.Count(); j++)
                {
                    if (meld[i].getColor() == meld[j].getColor())
                        return false;
                }
            }
            return true;
        }

        private void pool_btn_Click(object sender, EventArgs e)
        {
            // generate a card to the last-empty place in the board
            bool found_last_empty_location = false;
            for (int i = 1; i >= 0 && !found_last_empty_location; i--)
            {
                for (int j = 9; j >= 0 && !found_last_empty_location; j--)
                {
                    if (humanPlayer.board.getTileButton_slot()[i, j].getState() == false)
                    {
                        int[] location_arr = { i, j };
                        humanPlayer.board.GenerateNewTile_byClickingPool(location_arr);
                        found_last_empty_location = true;

                        updatePoolSizeText();
                        Board.TAG_NUMBER++;
                    }

                }
            }
            Board.tookCard = true;
            developerData();
        }

        private void developerData()
        {
            string test = "";
            for (int i = 0; i < humanPlayer.board.getTilesDictionary().Keys.Count; i++)
            {
                test += "index: " + humanPlayer.board.getTilesDictionary().Keys.ElementAt(i) + ", " + humanPlayer.board.getTilesDictionary()[humanPlayer.board.getTilesDictionary().Keys.ElementAt(i)].ToString() + "\n";
            }
            data_indicator_2.Text = test;

            string test1 = "";
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    test1 += "[" + i + ", " + j + "]: " + humanPlayer.board.getTileButton_slot()[i, j].ToString() + "\n";
                }
            }
            data_indicator.Text = test1;
        }

        private void SortByValue_btn_Click(object sender, EventArgs e)
        {
            List<TileButton> sorted_cards = humanPlayer.board.getTilesDictionary().Values.ToList();
            sorted_cards = sorted_cards.OrderBy(card => card.getNumber()).ToList();
            humanPlayer.board.ArrangeCardsOnBoard(sorted_cards);
        }

        private void SortByColor_btn_Click(object sender, EventArgs e)
        {
            List<TileButton> sorted_cards = humanPlayer.board.getTilesDictionary().Values.ToList();
            sorted_cards = sorted_cards.OrderBy(card => card.getColor()).ToList();
            humanPlayer.board.ArrangeCardsOnBoard(sorted_cards);
        }
    }
}