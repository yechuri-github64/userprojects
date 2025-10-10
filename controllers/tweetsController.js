try {
  const env = process.env;
  const modelVersion = env.MODEL_VERSION || '1.0.0';
  const thresholdEnv = parseFloat(env.VIRAL_THRESHOLD || '0.5');

  const predict = async (req, res) => {
    try {
      const body = req.body || {};
      const tweet_text = body.tweet_text;

      if (!tweet_text) {
        const errObj = { error: { message: 'tweet_text is required', code: 'VALIDATION_ERROR', details: { field: 'tweet_text' } } };
        console.error(' Failed', errObj);
        return res.status(400).json(errObj);
      }

      if (typeof tweet_text !== 'string' || tweet_text.length > 280) {
        const errObj = { error: { message: 'tweet_text must be a string with max 280 characters', code: 'VALIDATION_ERROR', details: { field: 'tweet_text' } } };
        console.error(' Failed', errObj);
        return res.status(400).json(errObj);
      }

      const has_media = Boolean(body.has_media);
      const hashtags = Array.isArray(body.hashtags) ? body.hashtags : [];
      const mentions = Array.isArray(body.mentions) ? body.mentions : [];
      const followers = Number(body.author_followers_count) || 0;
      const reply_count = Number(body.reply_count) || 0;
      const retweet_count = Number(body.retweet_count) || 0;
      const like_count = Number(body.like_count) || 0;
      const quote_count = Number(body.quote_count) || 0;

      // Simple deterministic scoring heuristic
      let score = 0;
      score += Math.min(30, (tweet_text.length / 280) * 30); // length contribution
      if (has_media) score += 20;
      score += Math.min(25, hashtags.length * 4);
      score += Math.min(20, mentions.length * 3);
      score += Math.min(40, Math.log10(followers + 1) * 10);
      score += Math.min(20, Math.log10(retweet_count + 1) * 5);
      score += Math.min(15, Math.log10(like_count + 1) * 3);

      const viral_score = Math.round(Math.max(0, Math.min(100, score)));
      const probability = Math.max(0, Math.min(1, viral_score / 100));
      const threshold = isNaN(thresholdEnv) ? 0.5 : thresholdEnv;
      const will_go_viral = probability >= threshold;

      const top_reasons = [];
      if (has_media) top_reasons.push('Contains media');
      if (hashtags.length) top_reasons.push('Hashtags increase discoverability');
      if (mentions.length) top_reasons.push('Includes mentions which can boost engagement');
      if (followers > 1000) top_reasons.push('Author has a sizable follower base');
      if (retweet_count > 0) top_reasons.push('Already receiving retweets');

      const feature_importance = {
        tweet_text_length: 0.25,
        has_media: 0.2,
        hashtags_count: 0.15,
        mentions_count: 0.1,
        author_followers_count: 0.2,
        engagement_counts: 0.1
      };

      const suggested_actions = [];
      if (!has_media) suggested_actions.push('Add an image or video to increase engagement');
      if (hashtags.length < 2) suggested_actions.push('Use 1-3 relevant hashtags to improve discoverability');
      if (mentions.length < 1) suggested_actions.push('Mention relevant accounts to prompt resharing');
      if (tweet_text.length < 100) suggested_actions.push('Make the opening lines more attention-grabbing');

      const words = tweet_text.split(/\s+/).filter(Boolean).slice(0, 10);
      const salient_terms = words.map(w => w.replace(/[^\w#@]/g, '')).filter(Boolean);

      const attention_highlights = [];
      hashtags.slice(0, 3).forEach((h, i) => attention_highlights.push({ span: `#${h}`, weight: 0.6 - i * 0.1 }));
      mentions.slice(0, 3).forEach((m, i) => attention_highlights.push({ span: `@${m}`, weight: 0.5 - i * 0.1 }));

      const response = {
        will_go_viral,
        probability,
        viral_score,
        threshold,
        top_reasons,
        feature_importance,
        suggested_actions,
        model_version: modelVersion,
        explainability: {
          salient_terms,
          attention_highlights
        }
      };

      return res.json(response);
    } catch (err) {
      console.error(' Failed', err);
      return res.status(500).json({ error: { message: 'Internal error', code: 'INTERNAL_ERROR', details: err.message } });
    }
  };

  console.log(' Connected');
  module.exports = { predict };
} catch (err) {
  console.error(' Failed', err);
  throw err;
}
