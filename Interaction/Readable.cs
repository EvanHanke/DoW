using UnityEngine;

public class Readable : InteractionScript {
	public Book book;

	public override void OnInteract(){
		BookUI.ShowBook(book);
	}

	public override string LabelDesc()
	{
		return "Read: " + book.name;
	}
}
