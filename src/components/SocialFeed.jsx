import React from 'react'
import SocialEmbed, { TwitterEmbed, YouTubeEmbed } from './SocialEmbed'

// Configure these via Vite env (import.meta.env.VITE_TWITTER_TWEET_URL, VITE_YOUTUBE_VIDEO)
const TWITTER_TWEET = import.meta.env.VITE_TWITTER_TWEET_URL || ''
const YOUTUBE_VIDEO = import.meta.env.VITE_YOUTUBE_VIDEO_ID || ''

export default function SocialFeed(){
  return (
    <div className="space-y-3">
      <h4 className="font-semibold">Social Feed</h4>
      <p className="text-sm text-gray-500">Live embeds shown when environment variables are set (VITE_TWITTER_TWEET_URL, VITE_YOUTUBE_VIDEO_ID).</p>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {TWITTER_TWEET ? <TwitterEmbed tweetUrl={TWITTER_TWEET} /> : <a href="https://twitter.com/newkenya" className="p-3 bg-white rounded card-shadow">Twitter · @newkenya</a>}
        {YOUTUBE_VIDEO ? <YouTubeEmbed videoId={YOUTUBE_VIDEO} /> : <a href="https://www.youtube.com/newkenya" className="p-3 bg-white rounded card-shadow">YouTube · New Kenya</a>}
      </div>
    </div>
  )
}
