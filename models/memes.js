try {
  const memes = [
    {
      "id": "abc123",
      "title": "Distracted Boyfriend Meme",
      "imageUrl": "https://example.com/memes/abc123.jpg",
      "sourceUrl": "https://imgur.com/abc123",
      "author": "meme_creator",
      "tags": ["distracted-boyfriend","relatable","2020s"],
      "createdAt": "2025-10-10T12:34:56Z",
      "width": 1200,
      "height": 800,
      "nsfw": false,
      "altText": "A man looking at another woman while his girlfriend looks annoyed"
    },
    {
      "id": "def456",
      "title": "Drake Hotline Bling",
      "imageUrl": "https://example.com/memes/def456.jpg",
      "sourceUrl": "https://imgur.com/def456",
      "author": "drakefan",
      "tags": ["drake","mood","2010s"],
      "createdAt": "2024-06-01T09:00:00Z",
      "width": 1200,
      "height": 1200,
      "nsfw": false,
      "altText": "Drake disapproves then approves"
    },
    {
      "id": "ghi789",
      "title": "Two Buttons",
      "imageUrl": "https://example.com/memes/ghi789.jpg",
      "sourceUrl": "https://imgur.com/ghi789",
      "author": "meme_artist",
      "tags": ["two-buttons","decision","classic"],
      "createdAt": "2023-01-15T08:30:00Z",
      "width": 800,
      "height": 600,
      "nsfw": false,
      "altText": "Person sweating over choosing between two buttons"
    }
  ];
  function getAll() {
    return memes;
  }
  function getRandom() {
    const list = getAll();
    if (!list || list.length === 0) return null;
    return list[Math.floor(Math.random() * list.length)];
  }
  module.exports = { getAll, getRandom };
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
