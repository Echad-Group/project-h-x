import React, { useEffect, useRef } from 'react';

export function TwitterEmbed({ tweetUrl }) {
  const ref = useRef(null);

  useEffect(() => {
    if (!tweetUrl) return;
    // load twitter widgets.js if not present
    if (!window.twttr) {
      const s = document.createElement('script');
      s.src = 'https://platform.twitter.com/widgets.js';
      s.async = true;
      document.body.appendChild(s);
    } else {
      window.twttr.widgets.load(ref.current);
    }
  }, [tweetUrl]);

  return (
    <div ref={ref} className="bg-white rounded p-2 card-shadow">
      {tweetUrl ? (
        <blockquote className="twitter-tweet">
          <a href={tweetUrl}></a>
        </blockquote>
      ) : (
        <div className="text-sm text-gray-500">No tweet configured</div>
      )}
    </div>
  );
}

export function YouTubeEmbed({ videoId }) {
  if (!videoId) return <div className="bg-white rounded p-2 card-shadow text-sm text-gray-500">No video configured</div>;
  const src = `https://www.youtube.com/embed/${videoId}`;
  return (
    <div className="aspect-w-16 aspect-h-9">
      <iframe title="youtube-embed" src={src} frameBorder="0" allowFullScreen className="w-full h-full rounded" />
    </div>
  );
}

export default function SocialEmbed(props) {
  if (props.tweetUrl) return <TwitterEmbed tweetUrl={props.tweetUrl} />;
  if (props.videoId) return <YouTubeEmbed videoId={props.videoId} />;
  return <div className="bg-white rounded p-3 text-sm text-gray-500">No embed configured</div>;
}
