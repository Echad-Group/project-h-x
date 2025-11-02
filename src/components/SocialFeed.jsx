import React from 'react'

export default function SocialFeed(){
  return (
    <div className="space-y-3">
      <h4 className="font-semibold">Social Feed (Preview)</h4>
      <p className="text-sm text-gray-500">Live embeds require platform API keys. This is a placeholder with quick links.</p>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-2">
        <a href="https://twitter.com/newkenya" className="p-3 bg-white rounded card-shadow">Twitter · @newkenya</a>
        <a href="https://facebook.com/newkenya" className="p-3 bg-white rounded card-shadow">Facebook · /newkenya</a>
        <a href="https://instagram.com/newkenya" className="p-3 bg-white rounded card-shadow">Instagram · @newkenya</a>
      </div>
    </div>
  )
}
