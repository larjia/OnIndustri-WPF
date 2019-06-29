using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.Data.Model.MasterData
{
    public class Address : BindableBase
    {
        public int Id { get; set; }

        // 别名
        private string _shortName;
        public string ShortName
        {
            get { return _shortName; }
            set { SetProperty(ref _shortName, value); }
        }

        // 联系人
        private string _contact;
        public string Contact
        {
            get { return _contact; }
            set { SetProperty(ref _contact, value); }
        }

        // 联系电话
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        // 国家
        private string _country;
        public string Country
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }

        // 省份
        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        // 城市
        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        // 地址
        private string _addressLine;
        public string AddressLine
        {
            get { return _addressLine; }
            set { SetProperty(ref _addressLine, value); }
        }

        // 邮编
        private string _postCode;
        public string PostCode
        {
            get { return _postCode; }
            set { SetProperty(ref _postCode, value); }
        }

        // 内部类型
        private string _internalType;
        public string InternalType
        {
            get { return _internalType; }
            set { SetProperty(ref _internalType, value); }
        }

        // 默认
        private bool _isDefault;
        public bool IsDefault
        {
            get { return _isDefault; }
            set { SetProperty(ref _isDefault, value); }
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
