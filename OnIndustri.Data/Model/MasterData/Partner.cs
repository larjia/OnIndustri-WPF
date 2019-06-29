using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.Data.Model.MasterData
{
    public abstract class Partner : BindableBase
    {
        public int Id { get; set; }

        // 编码
        private string _number;
        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        // 名称(与开票资料一致)
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        // 地址
        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        // 地址2
        private string _address2;
        public string Address2
        {
            get { return _address2; }
            set { SetProperty(ref _address2, value); }
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

        // 类型
        private string _partnerType;
        public string PartnerType
        {
            get { return _partnerType; }
            set { SetProperty(ref _partnerType, value); }
        }

        // 内部类型
        private string _internalType;
        public string InternalType
        {
            get { return _internalType; }
            set { SetProperty(ref _internalType, value); }
        }

        // 默认货币
        private string _currency;
        public string Currency
        {
            get { return _currency; }
            set { SetProperty(ref _currency, value); }
        }

        // 默认税率%
        private decimal _taxRate;
        public decimal TaxRate
        {
            get { return _taxRate; }
            set { SetProperty(ref _taxRate, value); }
        }

        // 应收应付账户(内部)
        private string _account;
        public string Account
        {
            get { return _account; }
            set { SetProperty(ref _account, value); }
        }

        // 支付方式
        private string _creditTerm;
        public string CreditTerm
        {
            get { return _creditTerm; }
            set { SetProperty(ref _creditTerm, value); }
        }

        // 纳税人识别号(与开票资料一致)
        private string _tin;
        public string TIN
        {
            get { return _tin; }
            set { SetProperty(ref _tin, value); }
        }

        // 地址、电话(与开票资料一致)
        private string _addressAndPhone;
        public string AddressAndPhone
        {
            get { return _addressAndPhone; }
            set { SetProperty(ref _addressAndPhone, value); }
        }

        // 开户行及账户(与开票资料一致)
        private string _bankAccount;
        public string BankAccount
        {
            get { return _bankAccount; }
            set { SetProperty(ref _bankAccount, value); }
        }

        // 备注
        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        // 停用
        private bool _disabled;
        public bool Disable
        {
            get { return _disabled; }
            set { SetProperty(ref _disabled, value); }
        }

        // 联系人
        public virtual ObservableCollection<Contact> Contacts { get; set; }
        // 地址
        public virtual ObservableCollection<Address> Addresses { get; set; }
    }
}
