//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace КалькуляторДляЛитейщиков.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class СоставПродукции
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public СоставПродукции()
        {
            this.Заявки = new HashSet<Заявки>();
        }
    
        public int ID { get; set; }
        public int КодПродукции { get; set; }
        public int КодМатериала { get; set; }
    
        public virtual Материалы Материалы { get; set; }
        public virtual Продукции Продукции { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Заявки> Заявки { get; set; }
    }
}
