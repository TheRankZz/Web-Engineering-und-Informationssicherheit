using ServiceProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.DAO
{
    public class UserDAO
    {
        private ServiceProviderContext db;

        private static UserDAO instance;

        private UserDAO() {
            db = new ServiceProviderContext();
        }

        public static UserDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserDAO();
                }
                return instance;
            }
        }

        public User findByUsername(String username)
        {
            var u = db.Users
                .Where(c => c.username == username)
                .FirstOrDefault();
            return u;
        }

        public bool insert(User u)
        {
            db.Users.Add(u);
            db.SaveChanges();
            return findByUsername(u.username) != null ? true : false;
        }

        public bool isExists(string username)
        {
            var u = findByUsername(username);
            if (u != null) return true;
            return false;
        }

        public bool delete(int id)
        {
            var u = db.Users.Find(id);
            return delete(u);
        }


        public bool delete(string username)
        {
            var u = findByUsername(username);
            return delete(u);
        }


        public bool delete(User user)
        {
            if(user != null)
            {
                int id = user.Id;

                db.Users.Remove(user);
                db.SaveChanges();

                return (db.Users.Find(id) == null) ? true : false;
            }
            return false;
        }


        public int addMessage(User u, Message m)
        {
            u.messages.Add(m);
            return db.SaveChanges();
        }

        public int removeMessage(User u, Message m)
        {
            db.Messages.Remove(m);
            return db.SaveChanges();
        }

        public Message getFirstMessage(User u)
        {
            Message msg = u.messages.FirstOrDefault();
            return msg;
        }
       
    }
}                                            