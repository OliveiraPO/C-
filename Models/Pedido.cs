namespace Program.Models{
    public record Pedido{
        public Guid Id{get; init;}
        public string Restaurante{get;set;}
        public string Prato{get; set;}
        public int Quantidade{get; set;}
        
        public Pedido(Guid id, string restaurante, string prato, int quantidade){
            Id=id;
            Restaurante=restaurante;
            Prato=prato;
            Quantidade=quantidade;
        }
    }
    public class RestaurantePatchModel{
        public string restaurante{get;set;}="";
    }
}