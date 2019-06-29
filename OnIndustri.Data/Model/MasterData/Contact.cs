using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.Data.Model.MasterData
{
    public class Contact : BindableBase
    {
        public int Id { get; set; }

        // 姓名
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        // 职务
        private string _position;
        public string Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        // 邮件
        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        // 电话
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        // 手机
        private string _mobilePhone;
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { SetProperty(ref _mobilePhone, value); }
        }

        // QQ
        private string _qq;
        public string QQ
        {
            get { return _qq; }
            set { SetProperty(ref _qq, value); }
        }

        // 微信
        private string _wechat;
        public string WeChat
        {
            get { return _wechat; }
            set { SetProperty(ref _wechat, value); }
        }

        // 合作伙伴Id
        private int _partnerId;
        public int PartnerId
        {
            get { return _partnerId; }
            set { SetProperty(ref _partnerId, value); }
        }

        public virtual Partner Partner { get; set; }
    }
}
