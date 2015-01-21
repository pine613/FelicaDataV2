﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaData
{
    public partial class UserData
    {
		/// <summary>
		/// 購入処理
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="money"></param>
		/// <returns></returns>
        public bool Buy(
            int userId, 
            int money,
            int performerUserId = 0
            )
        {
            if (money < 0) { return false; }
            return this.MoneyExecute(userId, -money, performerUserId, true);
        }

        /// <summary>
        /// チャージ処理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public bool Charge(
            int userId,
            int money,
            int performerUserId = 0
            )
        {
            if (money < 0) { return false; }
            return this.MoneyExecute(userId, money, performerUserId, false);
        }

        /// <summary>
        /// 出金処理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public bool Withdraw(
            int userId,
            int money,
            int performerUserId = 0
            )
        {
            if (money < 0) { return false; }
            return this.MoneyExecute(userId, -money, performerUserId, false);
        }

        /// <summary>
        /// 金銭処理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        private bool MoneyExecute(
            int userId,
            int money,
            int performerUserId = 0,
            bool isBuy = false
            )
        {
            try
            {
                User performerUser = null;
                var user = this.GetUser(userId);

                // 実行者が未指定な場合、本人の購入とみなす
                // 他者の購入
                if (performerUserId != 0)
                {
                    performerUser = this.GetUser(performerUserId);
                }
                else
                {
                    // 本人の購入
                    performerUserId = userId;
                    performerUser = user;
                }

                if (user != null && performerUser != null)
                {
                    // 履歴の追加
                    var history = new FelicaData.MoneyHistory
                    {
                        UserId = userId,
                        PerformerUserId = performerUserId,
                        Money = money,
                        IsBuy = isBuy
                    };

                    user.Money += money;

                    this.UpdateUser(user);
                    this.CreateMoneyHistory(history);

                    return true;
                }

                return false; // 失敗
            }
            catch (DatabaseException e)
            {
                Debug.WriteLine(e.Message);
            }

            return false; // 例外発生
        }

        public MoneyHistory GetMoneyHistory(int id)
        {
            return this.Query<MoneyHistory>(x => x.Id == id).FirstOrDefault();
        }

        public MoneyHistory CreateMoneyHistory(MoneyHistory history)
        {
            var user = this.GetUser(history.UserId);

            history.CreatedAt = DateTime.Now;

            if (user != null)
            {
                return this.Create(history);
            }
            
            return null;
        }

        public List<MoneyHistory> GetMoneyHistories(int userId)
        {
            return this.Query<MoneyHistory>(h => h.UserId == userId);
        }

        public void Cancel(MoneyHistory history)
        {
            var sameIdHistory = this.GetMoneyHistory(history.Id);
            var sameUser = this.GetUser(history.UserId);

            if (sameIdHistory == null)
            {
                throw new DatabaseException("無効な履歴です。");
            }

            if (sameIdHistory.IsCancel)
            {
                throw new DatabaseException("既にキャンセルされています。");
            }

            if (sameUser == null)
            {
                throw new DatabaseException("ユーザーが存在しません。");
            }

            if (sameIdHistory.PerformerUserId != sameIdHistory.UserId)
            {
                throw new DatabaseException("管理者による操作は取り消せません。");
            }

            // バリデーション通過

            sameUser.Money -= history.Money; // 取り消し (逆の処理)
            sameIdHistory.IsCancel = true;

            // 更新
            this.UpdateUser(sameUser);
            this.Update(sameIdHistory);
        }
    }
}