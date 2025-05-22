namespace food_ordering_system.v2.UI.Admin
{
    partial class MenuManagement
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuManagement));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRiceMeal = new System.Windows.Forms.Button();
            this.btnBreakfast = new System.Windows.Forms.Button();
            this.btnChicken = new System.Windows.Forms.Button();
            this.btnPasta = new System.Windows.Forms.Button();
            this.btnCombo = new System.Windows.Forms.Button();
            this.btnSnack = new System.Windows.Forms.Button();
            this.btnDessert = new System.Windows.Forms.Button();
            this.btnBeverage = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(416, 258);
            this.panel1.MinimumSize = new System.Drawing.Size(345, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1194, 622);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Aeonik TRIAL", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(459, 106);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(278, 38);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Aeonik TRIAL", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(410, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 34);
            this.label1.TabIndex = 2;
            this.label1.Text = "Menu Management";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(416, 106);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 38);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Aeonik TRIAL", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(1506, 106);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(104, 38);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRiceMeal
            // 
            this.btnRiceMeal.Location = new System.Drawing.Point(520, 198);
            this.btnRiceMeal.Name = "btnRiceMeal";
            this.btnRiceMeal.Size = new System.Drawing.Size(116, 37);
            this.btnRiceMeal.TabIndex = 5;
            this.btnRiceMeal.Text = "Rice Meal";
            this.btnRiceMeal.UseVisualStyleBackColor = true;
            // 
            // btnBreakfast
            // 
            this.btnBreakfast.Location = new System.Drawing.Point(654, 198);
            this.btnBreakfast.Name = "btnBreakfast";
            this.btnBreakfast.Size = new System.Drawing.Size(116, 37);
            this.btnBreakfast.TabIndex = 6;
            this.btnBreakfast.Text = "Breakfast";
            this.btnBreakfast.UseVisualStyleBackColor = true;
            this.btnBreakfast.Click += new System.EventHandler(this.btnBreakfast_Click);
            // 
            // btnChicken
            // 
            this.btnChicken.Location = new System.Drawing.Point(787, 198);
            this.btnChicken.Name = "btnChicken";
            this.btnChicken.Size = new System.Drawing.Size(144, 37);
            this.btnChicken.TabIndex = 7;
            this.btnChicken.Text = "Chicken Specialties";
            this.btnChicken.UseVisualStyleBackColor = true;
            // 
            // btnPasta
            // 
            this.btnPasta.Location = new System.Drawing.Point(946, 198);
            this.btnPasta.Name = "btnPasta";
            this.btnPasta.Size = new System.Drawing.Size(109, 37);
            this.btnPasta.TabIndex = 8;
            this.btnPasta.Text = "Pasta";
            this.btnPasta.UseVisualStyleBackColor = true;
            this.btnPasta.Click += new System.EventHandler(this.btnPasta_Click);
            // 
            // btnCombo
            // 
            this.btnCombo.Location = new System.Drawing.Point(1071, 198);
            this.btnCombo.Name = "btnCombo";
            this.btnCombo.Size = new System.Drawing.Size(120, 37);
            this.btnCombo.TabIndex = 9;
            this.btnCombo.Text = "Combo Meals";
            this.btnCombo.UseVisualStyleBackColor = true;
            // 
            // btnSnack
            // 
            this.btnSnack.Location = new System.Drawing.Point(1208, 198);
            this.btnSnack.Name = "btnSnack";
            this.btnSnack.Size = new System.Drawing.Size(116, 37);
            this.btnSnack.TabIndex = 10;
            this.btnSnack.Text = "Snacks";
            this.btnSnack.UseVisualStyleBackColor = true;
            // 
            // btnDessert
            // 
            this.btnDessert.Location = new System.Drawing.Point(1346, 198);
            this.btnDessert.Name = "btnDessert";
            this.btnDessert.Size = new System.Drawing.Size(116, 37);
            this.btnDessert.TabIndex = 11;
            this.btnDessert.Text = "Desserts";
            this.btnDessert.UseVisualStyleBackColor = true;
            this.btnDessert.Click += new System.EventHandler(this.btnDessert_Click);
            // 
            // btnBeverage
            // 
            this.btnBeverage.Location = new System.Drawing.Point(1481, 198);
            this.btnBeverage.Name = "btnBeverage";
            this.btnBeverage.Size = new System.Drawing.Size(116, 37);
            this.btnBeverage.TabIndex = 12;
            this.btnBeverage.Text = "Beverages";
            this.btnBeverage.UseVisualStyleBackColor = true;
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(428, 198);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(75, 37);
            this.btnAll.TabIndex = 13;
            this.btnAll.Text = "All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // MenuManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnBeverage);
            this.Controls.Add(this.btnDessert);
            this.Controls.Add(this.btnSnack);
            this.Controls.Add(this.btnCombo);
            this.Controls.Add(this.btnPasta);
            this.Controls.Add(this.btnChicken);
            this.Controls.Add(this.btnBreakfast);
            this.Controls.Add(this.btnRiceMeal);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(1706, 0);
            this.Name = "MenuManagement";
            this.Size = new System.Drawing.Size(1706, 894);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRiceMeal;
        private System.Windows.Forms.Button btnBreakfast;
        private System.Windows.Forms.Button btnChicken;
        private System.Windows.Forms.Button btnPasta;
        private System.Windows.Forms.Button btnCombo;
        private System.Windows.Forms.Button btnSnack;
        private System.Windows.Forms.Button btnDessert;
        private System.Windows.Forms.Button btnBeverage;
        private System.Windows.Forms.Button btnAll;
    }
}
