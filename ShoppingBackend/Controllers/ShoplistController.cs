using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingBackend.Models;

namespace ShoppingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoplistController : ControllerBase
    {
        private readonly ShoplistDbContext db;

        public ShoplistController(ShoplistDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gets all shopping list items.
        /// </summary>
        /// <returns>An HTTP 200 response containing the shopping list items.</returns>
        [HttpGet]
        public ActionResult Get()
        {
            var items = db.Shoplist.ToList();
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a shoplist item by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the shoplist item to retrieve.</param>
        /// <returns>An <see cref="OkObjectResult"/> containing the item if found; otherwise, a <see cref="NotFoundResult"/>.</returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var item = db.Shoplist.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a shoplist item.
        /// </summary>
        /// <param name="item">The shoplist item to create.</param>
        /// <returns>An action result containing the created item and its location.</returns>
        [HttpPost]
        public ActionResult Post([FromBody] Shoplist item)
        {
            db.Shoplist.Add(item);
            db.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        /// <summary>
        /// Updates the specified shop list.
        /// </summary>
        /// <param name="id">The identifier of the shop list to update.</param>
        /// <param name="item">The updated shop list.</param>
        /// <returns>A 204 No Content response if the update succeeds; otherwise, a 400 Bad Request response if the identifier
        /// does not match the shop list.</returns>
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Shoplist item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Deletes the shoplist item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the shoplist item to delete.</param>
        /// <returns>A 204 No Content response if the item is deleted; otherwise, a 404 Not Found response.</returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var item = db.Shoplist.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            db.Shoplist.Remove(item);
            db.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Searches for shoplist entries whose item text contains the specified text, ignoring case.
        /// </summary>
        /// <param name="item">The item text to search for.</param>
        /// <returns>An HTTP 200 response containing the matching shoplist entries.</returns>
        [HttpPost("search")]
        public ActionResult Search([FromBody] Shoplist item)
        {
            var results = db.Shoplist
                .Where(s => s.Item.ToLower().Contains(item.Item.ToLower()))
                .ToList();
            return Ok(results);
        }

        /// <summary>
        /// Retrieves shoplist entries with amounts within the specified range.
        /// </summary>
        /// <param name="minAmount">The minimum amount to include, inclusive.</param>
        /// <param name="maxAmount">The maximum amount to include, inclusive.</param>
        /// <returns>An HTTP 200 response containing the matching shoplist entries.</returns>
        [HttpGet("filter")]
        public ActionResult Filter(int minAmount, int maxAmount)
        {
            var results = db.Shoplist
                .Where(s => s.Amount >= minAmount && s.Amount <= maxAmount)
                .ToList();
            return Ok(results);
        }

        /// <summary>
        /// Returns the shoplist entries sorted by the specified property.
        /// </summary>
        /// <param name="sortBy">The name of the property to sort by.</param>
        /// <returns>An HTTP 200 response containing the sorted shoplist entries.</returns>
        [HttpGet("sort")]
        public ActionResult Sort(string sortBy)
        {
            var results = db.Shoplist
                .OrderBy(s => EF.Property<object>(s, sortBy))
                .ToList();
            return Ok(results);
        }

        /// <summary>
        /// Retrieves a page of shoplist entries.
        /// </summary>
        /// <param name="pageNumber">The one-based page number to retrieve.</param>
        /// <param name="pageSize">The number of entries to include in the page.</param>
        /// <returns>An action result containing the requested shoplist entries.</returns>
        [HttpGet("paginate")]
        public ActionResult Paginate(int pageNumber, int pageSize)
        {
            var results = db.Shoplist
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(results);
        }

        /// <summary>
        /// Gets the number of items in the shoplist.
        /// </summary>
        /// <returns>An HTTP 200 response containing the shoplist item count.</returns>
        [HttpGet("count")]
        public ActionResult Count()
        {
            var count = db.Shoplist.Count();
            return Ok(count);
        }

        /// <summary>
        /// Checks whether a shoplist exists.
        /// </summary>
        /// <param name="id">The identifier of the shoplist to check.</param>
        /// <returns>An HTTP 200 response containing true if the shoplist exists; otherwise, false.</returns>
        [HttpGet("exists/{id}")]
        public ActionResult Exists(int id)
        {
            var exists = db.Shoplist.Any(s => s.Id == id);
            return Ok(exists);
        }

        /// <summary>
        /// Retrieves the distinct items in the shop list.
        /// </summary>
        /// <returns>An HTTP 200 response containing the distinct shop list items.</returns>
        [HttpGet("distinct-items")]
        public ActionResult DistinctItems()
        {
            var items = db.Shoplist
                .Select(s => s.Item)
                .Distinct()
                .ToList();
            return Ok(items);
        }

        /// <summary>
        /// Groups shopping list entries by item and returns the count for each item.
        /// </summary>
        /// <returns>An HTTP 200 response containing each item and its count.</returns>
        [HttpGet("group-by-item")]
        public ActionResult GroupByItem()
        {
            var grouped = db.Shoplist
                .GroupBy(s => s.Item)
                .Select(g => new { Item = g.Key, Count = g.Count() })
                .ToList();
            return Ok(grouped);
        }

        /// <summary>
        /// Gets the maximum amount in the shoplist.
        /// </summary>
        /// <returns>An <see cref="OkObjectResult"/> containing the maximum amount.</returns>
        [HttpGet("max-amount")]
        public ActionResult MaxAmount()
        {
            var maxAmount = db.Shoplist.Max(s => s.Amount);
            return Ok(maxAmount);
        }

        /// <summary>
        /// Retrieves the smallest amount in the shoplist.
        /// </summary>
        /// <returns>An HTTP 200 response containing the smallest amount.</returns>
        [HttpGet("min-amount")]
        public ActionResult MinAmount()
        {
            var minAmount = db.Shoplist.Min(s => s.Amount);
            return Ok(minAmount);
        }

        /// <summary>
        /// Gets the average amount across all shoplist items.
        /// </summary>
        /// <returns>An HTTP 200 response containing the average amount.</returns>
        [HttpGet("average-amount")]
        public ActionResult AverageAmount()
        {
            var averageAmount = db.Shoplist.Average(s => s.Amount);
            return Ok(averageAmount);
        }

        /// <summary>
        /// Calculates the sum of all shop list amounts.
        /// </summary>
        /// <returns>An HTTP 200 response containing the sum.</returns>
        [HttpGet("sum-amount")]
        public ActionResult SumAmount()
        {
            var sumAmount = db.Shoplist.Sum(s => s.Amount);
            return Ok(sumAmount);
        }

        /// <summary>
        /// Retrieves the first shoplist entry.
        /// </summary>
        /// <returns>An HTTP 200 response containing the first shoplist entry, or null if the list is empty.</returns>
        [HttpGet("first")]
        public ActionResult First()
        {
            var first = db.Shoplist.FirstOrDefault();
            return Ok(first);
        }

        /// <summary>
        /// Retrieves the shopping list with the highest ID.
        /// </summary>
        /// <returns>An HTTP 200 response containing the shopping list with the highest ID, or null if no shopping lists exist.</returns>
        [HttpGet("last")]
        public ActionResult Last()
        {
            var last = db.Shoplist.OrderByDescending(s => s.Id).FirstOrDefault();
            return Ok(last);
        }

        /// <summary>
        /// Retrieves a shoplist item by its ID.
        /// </summary>
        /// <param name="id">The ID of the shoplist item.</param>
        /// <returns>An HTTP 200 response containing the matching item, or null if no item is found.</returns>
        [HttpGet("single/{id}")]
        public ActionResult Single(int id)
        {
            var single = db.Shoplist.SingleOrDefault(s => s.Id == id);
            return Ok(single);
        }

        /// <summary>
        /// Determines whether any shopping lists exist.
        /// </summary>
        /// <returns>An HTTP 200 response containing <see langword="true"/> if at least one shopping list exists; otherwise, <see
        /// langword="false"/>.</returns>
        [HttpGet("any")]
        public ActionResult Any()
        {
            var any = db.Shoplist.Any();
            return Ok(any);
        }

        /// <summary>
        /// Retrieves all shoplist items with an amount greater than zero.
        /// </summary>
        /// <returns>An HTTP 200 response containing the matching shoplist items.</returns>
        [HttpGet("all")]
        public ActionResult All()
        {
            var all = db.Shoplist.All(s => s.Amount > 0);
            return Ok(all);
        }

        /// <summary>
        /// Determines whether the shopping list contains the specified item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>An HTTP 200 response containing true if the item is present; otherwise, false.</returns>
        [HttpGet("contains/{item}")]
        public ActionResult Contains(string item)
        {
            var contains = db.Shoplist.Any(s => s.Item.ToLower() == item.ToLower());
            return Ok(contains);
        }

        /// <summary>
        /// Finds shopping-list entries whose item name matches the specified value, ignoring case.
        /// </summary>
        /// <param name="item">The item name to search for.</param>
        /// <returns>An HTTP 200 response containing the matching shopping-list entries.</returns>
        [HttpGet("find/{item}")]
        public ActionResult Find(string item)
        {
            var found = db.Shoplist.Where(s => s.Item.ToLower() == item.ToLower()).ToList();
            return Ok(found);
        }

        /// <summary>
        /// Finds the first shopping-list entry matching the specified item.
        /// </summary>
        /// <param name="item">The item name to search for.</param>
        /// <returns>An HTTP 200 response containing the matching entry, or null if no entry is found.</returns>
        [HttpGet("find-first/{item}")]
        public ActionResult FindFirst(string item)
        {
            var found = db.Shoplist.FirstOrDefault(s => s.Item.ToLower() == item.ToLower());
            return Ok(found);
        }

        /// <summary>
        /// Finds the most recent shopping-list entry matching the specified item.
        /// </summary>
        /// <param name="item">The item name to search for.</param>
        /// <returns>An HTTP 200 response containing the matching entry, or null if no entry is found.</returns>
        [HttpGet("find-last/{item}")]
        public ActionResult FindLast(string item)
        {
            var found = db.Shoplist.OrderByDescending(s => s.Id).FirstOrDefault(s => s.Item.ToLower() == item.ToLower());
            return Ok(found);
        }

        /// <summary>
        /// Finds a shopping list item by name, ignoring case.
        /// </summary>
        /// <param name="item">The name of the item to find.</param>
        /// <returns>An HTTP 200 response containing the matching item, or null if no item is found.</returns>
        [HttpGet("find-single/{item}")]
        public ActionResult FindSingle(string item)
        {
            var found = db.Shoplist.SingleOrDefault(s => s.Item.ToLower() == item.ToLower());
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shopping-list entry matches the specified item, ignoring case.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>An HTTP 200 response containing <see langword="true"/> if a matching entry exists; otherwise, <see
        /// langword="false"/>.</returns>
        [HttpGet("find-any/{item}")]
        public ActionResult FindAny(string item)
        {
            var found = db.Shoplist.Any(s => s.Item.ToLower() == item.ToLower());
            return Ok(found);
        }

        /// <summary>
        /// Finds shopping-list entries whose item name matches the specified value, ignoring case.
        /// </summary>
        /// <param name="item">The item name to search for.</param>
        /// <returns>An HTTP 200 response containing the matching shopping-list entries.</returns>
        [HttpGet("find-all/{item}")]
        public ActionResult FindAll(string item)
        {
            var found = db.Shoplist.Where(s => s.Item.ToLower() == item.ToLower()).ToList();
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item contains the specified text.
        /// </summary>
        /// <param name="item">The text to search for within shoplist items.</param>
        /// <returns>An HTTP 200 response containing a Boolean indicating whether a matching item was found.</returns>
        [HttpGet("find-contains/{item}")]
        public ActionResult FindContains(string item)
        {
            var found = db.Shoplist.Any(s => s.Item.ToLower().Contains(item.ToLower()));
            return Ok(found);
        }

        /// <summary>
        /// Checks whether any shoplist item starts with the specified text, ignoring case.
        /// </summary>
        /// <param name="item">The text to search for at the beginning of shoplist items.</param>
        /// <returns>An HTTP 200 response containing a Boolean indicating whether a matching item exists.</returns>
        [HttpGet("find-starts-with/{item}")]
        public ActionResult FindStartsWith(string item)
        {
            var found = db.Shoplist.Any(s => s.Item.ToLower().StartsWith(item.ToLower()));
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item ends with the specified text.
        /// </summary>
        /// <param name="item">The text to match at the end of a shoplist item.</param>
        /// <returns>An HTTP 200 response containing a Boolean indicating whether a matching item exists.</returns>
        [HttpGet("find-ends-with/{item}")]
        public ActionResult FindEndsWith(string item)
        {
            var found = db.Shoplist.Any(s => s.Item.ToLower().EndsWith(item.ToLower()));
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item matches an item in the comma-separated list.
        /// </summary>
        /// <param name="items">A comma-separated list of item names to search for.</param>
        /// <returns>An action result containing whether a matching shoplist item was found.</returns>
        [HttpGet("find-in/{items}")]
        public ActionResult FindIn(string items)
        {
            var itemList = items.Split(',').Select(i => i.ToLower()).ToList();
            var found = db.Shoplist.Any(s => itemList.Contains(s.Item.ToLower()));
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item is absent from the specified list.
        /// </summary>
        /// <param name="items">A comma-separated list of item names to check.</param>
        /// <returns>An action result containing <see langword="true"/> if any shoplist item is absent from the specified list;
        /// otherwise, <see langword="false"/>.</returns>
        [HttpGet("find-not-in/{items}")]
        public ActionResult FindNotIn(string items)
        {
            var itemList = items.Split(',').Select(i => i.ToLower()).ToList();
            var found = db.Shoplist.Any(s => !itemList.Contains(s.Item.ToLower()));
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shopping list item has an amount within the specified range.
        /// </summary>
        /// <param name="minAmount">The inclusive lower bound of the amount range.</param>
        /// <param name="maxAmount">The inclusive upper bound of the amount range.</param>
        /// <returns>An HTTP 200 response containing true if a shopping list item has an amount within the range; otherwise,
        /// false.</returns>
        [HttpGet("find-between/{minAmount}/{maxAmount}")]
        public ActionResult FindBetween(int minAmount, int maxAmount)
        {
            var found = db.Shoplist.Any(s => s.Amount >= minAmount && s.Amount <= maxAmount);
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item has an amount outside the specified range.
        /// </summary>
        /// <param name="minAmount">The minimum amount in the range.</param>
        /// <param name="maxAmount">The maximum amount in the range.</param>
        /// <returns>An HTTP 200 response containing true if any item has an amount below minAmount or above maxAmount;
        /// otherwise, false.</returns>
        [HttpGet("find-not-between/{minAmount}/{maxAmount}")]
        public ActionResult FindNotBetween(int minAmount, int maxAmount)
        {
            var found = db.Shoplist.Any(s => s.Amount < minAmount || s.Amount > maxAmount);
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shop list item has an amount greater than the specified amount.
        /// </summary>
        /// <param name="amount">The amount to compare against.</param>
        /// <returns>An HTTP 200 response containing <see langword="true"/> if a matching item exists; otherwise, <see
        /// langword="false"/>.</returns>
        [HttpGet("find-greater-than/{amount}")]
        public ActionResult FindGreaterThan(int amount)
        {
            var found = db.Shoplist.Any(s => s.Amount > amount);
            return Ok(found);
        }

        /// <summary>
        /// Indicates whether any shoplist item has an amount less than the specified value.
        /// </summary>
        /// <param name="amount">The amount to compare against.</param>
        /// <returns>An HTTP 200 response containing true if any shoplist item has an amount less than <paramref name="amount"/>,
        /// otherwise false.</returns>
        [HttpGet("find-less-than/{amount}")]
        public ActionResult FindLessThan(int amount)
        {
            var found = db.Shoplist.Any(s => s.Amount < amount);
            return Ok(found);
        }

        /// <summary>
        /// Indicates whether any shoplist item has an amount greater than or equal to the specified amount.
        /// </summary>
        /// <param name="amount">The minimum amount to search for.</param>
        /// <returns>An HTTP 200 response containing <see langword="true"/> if a matching item exists; otherwise, <see
        /// langword="false"/>.</returns>
        [HttpGet("find-greater-than-or-equal/{amount}")]
        public ActionResult FindGreaterThanOrEqual(int amount)
        {
            var found = db.Shoplist.Any(s => s.Amount >= amount);
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item has an amount less than or equal to the specified amount.
        /// </summary>
        /// <param name="amount">The amount to compare against.</param>
        /// <returns>An HTTP 200 response containing true if a matching item exists; otherwise, false.</returns>
        [HttpGet("find-less-than-or-equal/{amount}")]
        public ActionResult FindLessThanOrEqual(int amount)
        {
            var found = db.Shoplist.Any(s => s.Amount <= amount);
            return Ok(found);
        }

        /// <summary>
        /// Checks whether any shopping-list item has the specified amount.
        /// </summary>
        /// <param name="amount">The amount to search for.</param>
        /// <returns>An HTTP 200 response containing <see langword="true"/> if an item with the specified amount exists;
        /// otherwise, <see langword="false"/>.</returns>
        [HttpGet("find-equal/{amount}")]
        public ActionResult FindEqual(int amount)
        {
            var found = db.Shoplist.Any(s => s.Amount == amount);
            return Ok(found);
        }

        /// <summary>
        /// Determines whether any shoplist item has an amount different from the specified amount.
        /// </summary>
        /// <param name="amount">The amount to compare against.</param>
        /// <returns>An HTTP 200 response containing true if any shoplist item has a different amount; otherwise, false.</returns>
        [HttpGet("find-not-equal/{amount}")]
        public ActionResult FindNotEqual(int amount)
        {
            var found = db.Shoplist.Any(s => s.Amount != amount);
            return Ok(found);
        }
    }
}
