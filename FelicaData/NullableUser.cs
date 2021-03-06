﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaData
{
    public class NullableUser
    {
        public User User { get; set; }

        public string Id
        {
            get
            {
                if (this.User == null) { return null; }
                return this.User.Id;
            }
        }

        public string Name
        {
            get
            {
                if (this.User == null) { return string.Empty; }
                return this.User.Name;
            }
        }

        public string Email
        {
            get
            {
                if (this.User == null) { return string.Empty; }
                return this.User.Email;
            }
        }

        public int Money
        {
            get
            {
                if (this.User == null) { return 0; }
                return this.User.Money;
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (this.User == null) { return false; }
                return this.User.IsAdmin;
            }
        }

        public byte[] Avatar
        {
            get
            {
                if (this.User == null) { return null; }
                return this.User.Avatar;
            }
        }

        /// <summary>
        /// 寮の部屋番号
        /// </summary>
        public string DormitoryRoomNo
        {
            get
            {
                if (this.User == null) { return null; }
                return this.User.DormitoryRoomNumber;
            }
        }

        /// <summary>
        /// 電話番号
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                if (this.User == null) { return null; }
                return this.User.PhoneNumber;
            }
        }

    }
}