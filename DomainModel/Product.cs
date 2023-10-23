namespace DomainModel {
    public class Product {
        public int id { get; set; }

        public required string name { get; set; }
        
        public bool alcoholic { get; set; }

        private string? _imageUrl { get; set; }
        public string? imageUrl {
            get { return _imageUrl ?? "https://media.istockphoto.com/id/1147544807/fr/vectoriel/pas-image-miniature-graphique-vectoriel.jpg?s=612x612&w=is&k=20&c=-RnzwVmgpowhd-C1iGQS30Sd5FrvKCXPxfsCSVR-HSg="; }
            set { _imageUrl = value; }
        }
    }
}
