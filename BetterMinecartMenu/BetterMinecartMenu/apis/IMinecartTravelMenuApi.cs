#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.Minecarts;

namespace MinecartTravelMenu
{
    /// <summary>Provides API methods for integrating with and extending the Minecart Travel Menu mod functionality. This interface allows other mods to generate map thumbnails, manage destination previews, control menu behavior, handle favorites, and respond to menu events. Implementations are obtained through SMAPI's mod registry after the GameLaunched event.</summary>
    public interface IMinecartTravelMenuApi
    {
        /*********
         ** Rendering: the map / thumbnail
         *********/

        /// <summary>Retrieves the cached tilemap render for a specified location.</summary>
        /// <param name="location">The game location to get the map thumbnail for.</param>
        /// <return>The rendered texture of the location's map, or <c>null</c> if the location has no map or is too large to render.</return>
        Texture2D? GetMapThumbnail(GameLocation location);

        /// <summary>Retrieves a cached tilemap render for a location with custom rendering options.</summary>
        /// <param name="location">The game location to render a map thumbnail for.</param>
        /// <param name="maxEdgePixels">The maximum size in pixels for the longest edge of the rendered thumbnail.</param>
        /// <param name="detailed">Whether to render a detailed view including buildings and objects, or a tiles-only view.</param>
        /// <param name="includeCharacters">Whether to include characters in the rendered thumbnail.</param>
        /// <return>A cached texture containing the map thumbnail, or <c>null</c> if the location has no map or is too large to render.</return>
        Texture2D? GetMapThumbnail(GameLocation location, int maxEdgePixels, bool detailed, bool includeCharacters);

        /// <summary>The destination's map thumbnail plus a source rectangle zoomed and centered on the arrival tile, so a plain image widget can show the arrival area zoomed in (like MTM's cards, minus the marker) instead of the whole map. The crop preserves the texture's aspect ratio; returns <c>null</c> if the map can't be rendered. Bind the tuple to a StardewUI <c>&lt;image sprite&gt;</c> (it accepts <c>Tuple&lt;Texture2D, Rectangle&gt;</c>), or use it as the sourceRectangle in <c>SpriteBatch.Draw</c>. Renders per the player's config; call OUTSIDE an active SpriteBatch. The texture is cache-owned; do not dispose it.</summary>
        /// <param name="location">The game location to render a thumbnail for.</param>
        /// <param name="arrivalTile">The tile coordinates to center the zoomed view on.</param>
        /// <param name="zoom">The zoom multiplier for the centered crop area.</param>
        /// <return>A tuple containing the rendered texture and the source rectangle for the zoomed area, or <c>null</c> if rendering fails.</return>
        Tuple<Texture2D, Rectangle>? GetDestinationThumbnail(GameLocation location, Point arrivalTile, float zoom = 2f);

        /// <summary>Where the arrival tile sits within the crop returned by
        /// <see cref="GetDestinationThumbnail"/> (same <paramref name="location"/>,
        /// <paramref name="arrivalTile"/>, and <paramref name="zoom"/>), as a 0..1 fraction of the
        /// crop's width and height. Because the crop clamps at map edges, this is not always
        /// (0.5, 0.5): for an arrival tile near a border the crop shifts and the tile moves toward a
        /// corner. Multiply by the on-screen size you draw the thumbnail at to position a marker so it
        /// points at the real arrival spot instead of the center.</summary>
        /// <param name="location">The game location to calculate the marker offset for.</param>
        /// <param name="arrivalTile">The tile coordinates where the player would arrive.</param>
        /// <param name="zoom">The zoom level used for the thumbnail crop.</param>
        /// <return>A normalized vector (0..1) indicating the arrival tile's position within the thumbnail crop, or <c>null</c> in the same cases <see cref="GetDestinationThumbnail"/> returns <c>null</c>.</return>
        Vector2? GetDestinationMarkerOffset(GameLocation location, Point arrivalTile, float zoom = 2f);

        /// <summary>Resolve the destination preview for a location and arrival tile: a world-map
        /// crop with a bouncing arrow marker, a live tilemap snapshot where there is no world-map
        /// data, an optional region subtitle, and the time-of-day tint, following the mod's config.
        /// <para>Resolving renders the snapshot if one is needed, so call this OUTSIDE the draw loop
        /// (for example when you open your menu), hold the returned handle, and call
        /// <see cref="IMapPreview.Draw"/> on it each frame from your draw. This mirrors how the
        /// menu itself works and keeps a snapshot render from ever happening mid-batch.</para></summary>
        /// <param name="location">The destination game location.</param>
        /// <param name="arrivalTile">The tile coordinates where the player will arrive.</param>
        /// <param name="cardLabel">The title you draw beside the preview; it is used only to
        /// drop a region subtitle that would just repeat it, so pass <c>null</c> to keep every
        /// subtitle.</param>
        /// <return>The map preview handle for the destination. Never returns <c>null</c> (you get a
        /// placeholder preview if nothing resolved; see <see cref="IMapPreview.IsPlaceholder"/>).</return>
        IMapPreview ResolveDestinationPreview(GameLocation location, Point arrivalTile, string? cardLabel = null);

        /// <summary>Creates a resolved destination card for a minecart destination that can be drawn in custom menus.
        /// The card contains rendered previews and provides drawing methods for standard or custom layouts.
        /// This method renders the preview on first call, so it must be called OUTSIDE an active SpriteBatch.
        /// Returns <c>null</c> if the destination has no usable location.</summary>
        /// <param name="networkId">The network identifier used only to determine the card's favorite status.</param>
        /// <param name="destination">The minecart destination data to create a card for.</param>
        /// <return>A destination card that can be drawn via its Draw methods, or <c>null</c> if the destination location is not usable.</return>
        IDestinationCard? CreateDestinationCard(string networkId, MinecartDestinationData destination);

        /// <summary>
        /// Gets the sprite (texture and source rectangle) that MTM uses for the favorite star icon.
        /// </summary>
        /// <return>
        /// A tuple containing the texture and source rectangle for the favorite star sprite,
        /// formatted as <c>Tuple&lt;Texture2D, Rectangle&gt;</c> for binding to StardewUI's
        /// <c>&lt;image sprite&gt;</c> in declarative UIs. Returns <c>null</c> if the sprite
        /// is unavailable.
        /// </return>
        Tuple<Texture2D, Rectangle>? GetFavoriteStarSprite();

        /*********
         ** Query data + open the menu
         *********/

        /// <summary>Gets the network IDs that are currently available at the specified location.</summary>
        /// <param name="location">The game location to check for available minecart networks.</param>
        /// <return>A read-only list of network IDs that are unlocked and have at least one valid destination from the specified location. Returns an empty list if the world is not loaded.</return>
        IReadOnlyList<string> GetAvailableNetworks(GameLocation location);

        /// <summary>The valid, display-ready destinations for a network at a location, filtered exactly as the menu filters them: per-destination GameStateQuery conditions, id/target sanity, de-duplication, and player-built minecart stops (when the "Built Carts as Destinations" option is on). Empty when the network is unknown, locked, or has no usable stops. Not free: with built carts enabled it scans every location's buildings, so call it when you build your UI, not every frame. Some returned entries are synthetic built-cart stops (id prefixed <c>ringlord174.MinecartTravelMenu_Built_</c>, <c>Price</c> 0) that are not in <c>Data/Minecarts</c>. This is the canonical data view: it does NOT include stops other mods inject through <see cref="OnMenuBuilding"/> (those are applied only when a menu is actually built).</summary>
        /// <param name="location">The game location where the minecart network is being accessed from.</param>
        /// <param name="networkId">The identifier of the minecart network to retrieve destinations for.</param>
        /// <return>A read-only list of minecart destination data for the specified network, or an empty list if the network is unavailable or has no valid destinations.</return>
        IReadOnlyList<MinecartDestinationData> GetDestinations(GameLocation location, string networkId);

        /// <summary>Opens the grid menu for a network at a location, as if the player used a cart there.</summary>
        /// <param name="location">The game location where the menu is being opened from.</param>
        /// <param name="networkId">The identifier of the minecart network to display destinations for.</param>
        /// <param name="excludeDestinationId">The stop identifier to exclude from selection, typically the stop being boarded from. This stop will be shown greyed out or hidden depending on configuration.</param>
        /// <return><c>true</c> if the menu was successfully opened; <c>false</c> if the menu was declined because the grid menu is disabled in config, the player is mounted, or the network is unknown, locked, or empty.</return>
        bool OpenMenu(GameLocation location, string networkId, string? excludeDestinationId = null);

        /*********
         ** Menu ownership: let another mod provide the UI
         *********/

        /// <summary>Requests that Minecart Travel Menu suppress or resume its grid menu functionality to allow another mod to provide the minecart UI.</summary>
        /// <param name="requestingModId">The unique ID of the mod making the suppression request.</param>
        /// <param name="suppress">Whether to suppress the menu (true) or resume normal operation (false).</param>
        void SuppressMenu(string requestingModId, bool suppress);

        /// <summary>Whether MTM's grid menu is currently suppressed by any mod (see
        /// <see cref="SuppressMenu"/>).</summary>
        /// <return><c>true</c> if the menu is suppressed by any mod; otherwise, <c>false</c>.</return>
        bool IsMenuSuppressed();

        /*********
         ** Hooks: destination injection + travel veto
         *********/

        /// <summary>Registers a handler that is invoked when the minecart travel menu is being built, allowing modification of the available destinations before the menu is displayed.</summary>
        /// <param name="handler">The callback to invoke with a context object that allows adding, removing, or reordering destinations in the menu.</param>
        void OnMenuBuilding(Action<IMenuBuildingContext> handler);

        /// <summary>Register a handler called when the player picks a destination, before any ticket
        /// prompt or warp. Return <c>false</c> to cancel the trip (the menu stays open); return
        /// <c>true</c> to allow it. The first handler to return <c>false</c> wins. Register once;
        /// handlers persist for the session. A handler that throws is logged and skipped (treated as
        /// "allow").</summary>
        /// <param name="handler">The callback function that receives the destination context and returns whether to allow the trip.</param>
        void OnDestinationSelected(Func<IDestinationContext, bool> handler);

        /*********
         ** Favorites and travel state (the current player)
         *********/

        /// <summary>Whether the current player has starred this stop.</summary>
        /// <param name="networkId">The minecart network identifier.</param>
        /// <param name="destinationId">The destination identifier within the network.</param>
        /// <return><c>true</c> if the destination is marked as a favorite by the current player; otherwise, <c>false</c>.</return>
        bool IsFavorite(string networkId, string destinationId);

        /// <summary>Toggles the favorite status for a minecart destination for the current player and persists the change immediately.</summary>
        /// <param name="networkId">The identifier of the minecart network.</param>
        /// <param name="destinationId">The identifier of the destination within the network.</param>
        void ToggleFavorite(string networkId, string destinationId);

        /// <summary>Retrieves the destination identifier that the current player last traveled to on the specified minecart network.</summary>
        /// <param name="networkId">The network identifier to query for the last destination.</param>
        /// <return>The destination identifier of the last traveled destination on this network, or <c>null</c> if no destination has been recorded.</return>
        string? GetLastDestination(string networkId);
    }

    /// <summary>Represents a resolved map preview for a minecart destination that can be efficiently drawn each frame. This interface provides a pre-generated visual representation of a destination location that has already performed all necessary computation and texture generation. Instances should be obtained through the API's resolution methods and cached for repeated drawing, then re-resolved only when the underlying destination, season, or configuration changes.</summary>
    public interface IMapPreview
    {
        /// <summary>Draws the map preview into the specified target area with the given opacity.</summary>
        /// <param name="spriteBatch">The SpriteBatch used for rendering.</param>
        /// <param name="target">The rectangular area where the preview should be drawn.</param>
        /// <param name="alpha">The opacity level for drawing, where 1.0 is fully opaque and 0.0 is fully transparent. Defaults to 1.0.</param>
        void Draw(SpriteBatch spriteBatch, Rectangle target, float alpha = 1f);

        /// <summary>A descriptive text label for the preview, typically indicating the location name or region being shown, or <c>null</c> if no subtitle is available.</summary>
        string? Subtitle { get; }

        /// <summary>Indicates whether this preview is a placeholder fallback used when no proper map preview could be resolved for the requested destination.</summary>
        bool IsPlaceholder { get; }
    }

    /// <summary>Represents a resolved destination card that displays information about a minecart travel destination. This interface provides properties for destination details and methods for rendering the card or its individual visual components. Cards should be created once via CreateDestinationCard and then reused for rendering. All Draw methods operate on pre-resolved graphics and are safe to call within an active SpriteBatch. For declarative UI frameworks, use the read properties combined with GetDestinationThumbnail and GetFavoriteStarSprite methods from the main API instead of the Draw methods.</summary>
    public interface IDestinationCard
    {
        /// <summary>The display name of the destination, parsed from the tokenizable display name string, or the target location identifier if no display name is available.</summary>
        string Label { get; }

        /// <summary>A secondary descriptive text for the destination, typically indicating the location name or region, or <c>null</c> if no subtitle is available.</summary>
        string? Subtitle { get; }

        /// <summary>The cost in gold required to travel to this destination, or 0 if travel is free.</summary>
        int Price { get; }

        /// <summary>Gets a value indicating whether this destination has been marked as a favorite by the player.</summary>
        bool IsFavorite { get; }

        /// <summary>The visual map preview component for this destination, which can be rendered to show a minimap or location thumbnail of the destination area.</summary>
        IMapPreview Preview { get; }

        /// <summary>Draws the parchment background of the destination card into the specified area.</summary>
        /// <param name="spriteBatch">The SpriteBatch to use for rendering the background.</param>
        /// <param name="area">The rectangular area where the background should be drawn.</param>
        /// <param name="hovered">If true, renders the background with a hover highlight effect.</param>
        /// <param name="alpha">The opacity level for rendering, where 1.0 is fully opaque and 0.0 is fully transparent.</param>
        void DrawBackground(SpriteBatch spriteBatch, Rectangle area, bool hovered = false, float alpha = 1f);

        /// <summary>Draws the destination label text, wrapped and truncated as needed, centered within the specified area.</summary>
        /// <param name="spriteBatch">The SpriteBatch to use for rendering the label.</param>
        /// <param name="area">The rectangular area in which to draw the label text.</param>
        /// <param name="alpha">The opacity level for rendering, where 1.0 is fully opaque and 0.0 is fully transparent.</param>
        void DrawLabel(SpriteBatch spriteBatch, Rectangle area, float alpha = 1f);

        /// <summary>Draws the map preview portion of the destination card, including the map thumbnail, location marker, fallback graphics if applicable, tint overlay, and subtitle text.</summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        /// <param name="area">The rectangular area in which to render the preview.</param>
        /// <param name="alpha">The opacity level for rendering, ranging from 0 (transparent) to 1 (opaque). Default is 1.</param>
        void DrawPreview(SpriteBatch spriteBatch, Rectangle area, float alpha = 1f);

        /// <summary>Draws the ticket price as a bottom overlay strip in the specified area.</summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="area">The rectangular area where the price overlay should be drawn.</param>
        /// <param name="unaffordable">Whether to render the price in a style indicating the player cannot afford it.</param>
        /// <param name="alpha">The transparency level of the price overlay, ranging from 0 (fully transparent) to 1 (fully opaque).</param>
        void DrawPrice(SpriteBatch spriteBatch, Rectangle area, bool unaffordable = false, float alpha = 1f);

        /// <summary>Draws the "you are here" badge across the specified area.</summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="area">The rectangular area where the badge should be rendered.</param>
        /// <param name="alpha">The opacity level for the badge, where 1.0 is fully opaque and 0.0 is fully transparent. Defaults to 1.0.</param>
        void DrawYouAreHere(SpriteBatch spriteBatch, Rectangle area, float alpha = 1f);

        /// <summary>Draws the favorite star indicator, scaled to fill the specified area's width.</summary>
        /// <param name="spriteBatch">The SpriteBatch to use for rendering.</param>
        /// <param name="area">The rectangular area where the favorite star should be drawn, with the star scaled to match the area's width.</param>
        /// <param name="alpha">The opacity level for rendering the star, where 1.0 is fully opaque and 0.0 is fully transparent.</param>
        void DrawFavoriteStar(SpriteBatch spriteBatch, Rectangle area, float alpha = 1f);

        /// <summary>Draws the complete destination card with all standard components in MTM's default tile layout.</summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with.</param>
        /// <param name="bounds">The rectangular area in which to draw the destination card.</param>
        /// <param name="hovered">Whether to draw the card in a hovered state.</param>
        /// <param name="alpha">The opacity level for drawing the card, where 1.0 is fully opaque and 0.0 is fully transparent.</param>
        void Draw(SpriteBatch spriteBatch, Rectangle bounds, bool hovered = false, float alpha = 1f);
    }

    /// <summary>Provides mutable context for customizing the minecart destination menu during construction. Exposes the current location, network identifier, and destination list, along with methods to add or remove destinations before the menu opens. This context is passed to handlers registered via IMinecartTravelMenuApi.OnMenuBuilding.</summary>
    public interface IMenuBuildingContext
    {
        /// <summary>The game location from which the minecart menu is being opened.</summary>
        GameLocation Location { get; }

        /// <summary>The unique identifier of the minecart network associated with the menu being built, representing which set of connected minecart destinations the player is accessing.</summary>
        string NetworkId { get; }

        /// <summary>The read-only list of minecart destinations currently available in the menu being built, which can be modified using Add or Remove methods.</summary>
        IReadOnlyList<MinecartDestinationData> Destinations { get; }

        /// <summary>Adds a custom minecart destination to the menu being built.</summary>
        /// <param name="destination">The destination data to add. Must have non-blank <c>Id</c> and <c>TargetLocation</c> properties. If a destination with the same <c>Id</c> already exists, the addition will be ignored.</param>
        void Add(MinecartDestinationData destination);

        /// <summary>Removes all destinations with the specified identifier from the menu being built.</summary>
        /// <param name="destinationId">The identifier of the destination(s) to remove. Comparison is case-insensitive. If null, empty, or no matching destinations exist, no changes are made.</param>
        void Remove(string destinationId);
    }

    /// <summary>Represents the context of a minecart destination selection, providing information about the player's chosen travel destination including the target location, network identifier, and destination data. This context is passed to handlers registered via OnDestinationSelected to allow mods to inspect or respond to destination choices before the travel occurs.</summary>
    public interface IDestinationContext
    {
        /// <summary>The game location where the selected minecart destination is located.</summary>
        GameLocation Location { get; }

        /// <summary>The identifier of the minecart network through which the player is traveling, corresponding to the network ID defined in the game's minecart data.</summary>
        string NetworkId { get; }

        /// <summary>The minecart destination data selected by the player, containing configuration details such as the destination identifier, target location, cost, and other properties defined in the game's minecart destination data.</summary>
        MinecartDestinationData Destination { get; }
    }
}
